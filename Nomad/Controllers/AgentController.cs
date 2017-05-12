using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nomad.Controllers
{
    public class AgentController : Controller
    {
        public static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/servers")]
        public async Task<IActionResult> Servers()
        {
            var agentOperatorTask = GetAgentOperatorsAsync();

            var agents = await GetAgentsAsync();
            agents.Operator = await agentOperatorTask;

            return View("~/Views/Nomad/Servers.cshtml", agents);
        }

        [Route("/server")]
        public async Task<IActionResult> Agent(string ip)
        {
            var agent = await GetAgentAsync(ip);

            return View("~/Views/Nomad/Server.cshtml", agent);
        }

        public async Task<Agent> GetAgentsAsync()
        {
            Agent agent;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/agent/members"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                agent = JsonConvert.DeserializeObject<Agent>(result);
            }

            foreach (var member in agent.Members)
            {
                if (member.Status == "alive") { member.Up++; }
                if (member.Status == "dead") { member.Down++; }
            }

            return agent;
        }

        public async Task<Agent> GetAgentAsync(string ip)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://" + ip + ":4646/v1/agent/self"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                return JsonConvert.DeserializeObject<Agent>(result);
            }
        }

        public async Task<Operator> GetAgentOperatorsAsync()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/operator/raft/configuration"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Operator>(result);
            }

        }
    }
}
