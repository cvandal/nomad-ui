using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nomad.Extensions;

namespace Nomad.Controllers
{
    public class ServerController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private static HttpClient HttpClient = new HttpClient();

        [Route("/servers")]
        public async Task<IActionResult> Servers(int? page)
        {
            var serverOperatorTask = GetServerOperatorsAsync();

            var servers = await GetServersAsync();
            foreach (var server in servers)
            {
                server.Operator = await serverOperatorTask;
            }

            return View("~/Views/Nomad/Servers.cshtml", PaginatedList<Member>.CreateAsync(servers, page ?? 1, 15));
        }

        [Route("/server")]
        public async Task<IActionResult> Server(string ip)
        {
            var serverOperatorTask = GetServerOperatorsAsync();
            
            var server = await GetServerAsync(ip);
            server.Member.Operator = await serverOperatorTask;

            return View("~/Views/Nomad/Server.cshtml", server);
        }

        public async Task<List<Member>> GetServersAsync()
        {
            Server server;
            List<Member> members = new List<Member>();

            var result = await HttpClient.GetAsync(NomadUrl + "/v1/agent/members").Result.Content.ReadAsStringAsync();

            server = JsonConvert.DeserializeObject<Server>(result);

            foreach (var member in server.Members)
            {
                if (member.Status == "alive") { member.Up++; }
                if (member.Status == "dead") { member.Down++; }

                members.Add(member);
            }

            return members.OrderBy(m => m.Name).ToList();
        }

        public async Task<Server> GetServerAsync(string ip)
        {
            var result = await HttpClient.GetAsync("http://" + ip + ":4646/v1/agent/self").Result.Content.ReadAsStringAsync();
            ViewData["Json"] = JToken.Parse(result).ToString(Formatting.Indented);

            return JsonConvert.DeserializeObject<Server>(result);
        }

        public async Task<Operator> GetServerOperatorsAsync()
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/operator/raft/configuration").Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Operator>(result);
        }
    }
}
