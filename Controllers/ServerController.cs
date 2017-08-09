using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nomad.Extensions;
using Nomad.Models;

namespace Nomad.Controllers
{
    public class ServerController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [HttpGet("/servers")]
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/servers")]
        public async Task<JsonResult> GetServersAsJsonResult(string search)
        {
            var agents = await GetServersAsync();

            if (!String.IsNullOrEmpty(search))
            {
                agents = agents.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(agents);
        }

        [HttpGet("/server")]
        public IActionResult Server()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/server")]
        public async Task<JsonResult> Server(string ip)
        {
            var agent = await GetServerAsync(ip);
            agent.Member.Operator = await GetOperatorsAsync();

            return Json(agent);
        }

        public async Task<List<Member>> GetServersAsync()
        {
            Agent agent;
            Operator @operator = await GetOperatorsAsync();
            List<Member> members = new List<Member>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/agent/members"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                agent = JsonConvert.DeserializeObject<Agent>(result);

                foreach (var member in agent.Members)
                {
                    if (member.Status == "alive") { member.Up++; }
                    if (member.Status == "dead") { member.Down++; }
                    
                    foreach (var server in @operator.Servers)
                    {
                        if (member.Name == server.Node)
                        {
                            member.Voter = server.Voter;
                            member.Leader = server.Leader;
                        }
                    }

                    members.Add(member);
                }
            }

            return members.OrderBy(m => m.Name).ToList();
        }

        public async Task<Agent> GetServerAsync(string ip)
        {
            var address = ip.ConvertToFriendlyAddress();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(address + "/v1/agent/self"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Agent>(result);
            }
        }

        public async Task<Operator> GetOperatorsAsync()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/operator/raft/configuration"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Operator>(result);
            }
        }
    }
}
