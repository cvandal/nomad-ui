using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nomad.Controllers
{
    public class ServerController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/servers")]
        public async Task<IActionResult> Servers()
        {
            var serverOperatorTask = GetServerOperatorsAsync();

            var servers = await GetServersAsync();
            servers.Operator = await serverOperatorTask;

            return View("~/Views/Nomad/Servers.cshtml", servers);
        }

        [Route("/server")]
        public async Task<IActionResult> Server(string ip)
        {
            var serverOperatorTask = GetServerOperatorsAsync();
            
            var server = await GetServerAsync(ip);
            server.Operator = await serverOperatorTask;

            return View("~/Views/Nomad/Server.cshtml", server);
        }

        public async Task<Server> GetServersAsync()
        {
            Server server;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/agent/members"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                server = JsonConvert.DeserializeObject<Server>(result);
            }

            foreach (var member in server.Members)
            {
                if (member.Status == "alive") { member.Up++; }
                if (member.Status == "dead") { member.Down++; }
            }

            return server;
        }

        public async Task<Server> GetServerAsync(string ip)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://" + ip + ":4646/v1/agent/self"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                return JsonConvert.DeserializeObject<Server>(result);
            }
        }

        public async Task<Operator> GetServerOperatorsAsync()
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
