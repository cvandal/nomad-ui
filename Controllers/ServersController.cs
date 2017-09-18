using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nomad.Extensions;

namespace Nomad.Controllers
{
    public class ServersController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        
        public IActionResult Index()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET
        // /api/servers
        [HttpGet("api/[controller]")]
        public async Task<List<JObject>> Servers()
        {
            var servers = new List<JObject>();

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/agent/members"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await content);

                foreach (var server in json.Members)
                {
                    servers.Add(server);
                }
            }

            return servers.OrderBy(s => s["Name"]).ToList();
        }

        // GET
        // /api/servers/status
        [HttpGet("api/[controller]/[action]")]
        public async Task<JObject> Status()
        {
            dynamic servers = await Servers();
            var up = 0;
            var down = 0;

            foreach (var server in servers)
            {
                if (server.Status == "alive") { up++; }
                if (server.Status == "dead") { down++; }
            }

            dynamic status = new JObject();
            status.Up = up;
            status.Down = down;

            return status;
        }
    }
}
