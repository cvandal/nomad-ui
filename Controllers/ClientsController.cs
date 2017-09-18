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
    public class ClientsController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        
        public IActionResult Index()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET
        // /api/clients
        [HttpGet("api/[controller]")]
        public async Task<List<JObject>> Clients()
        {
            var clients = new List<JObject>();

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(NomadUrl + "/v1/nodes"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await content);
                
                foreach (var client in json)
                {
                    clients.Add(client);
                }
            }

            return clients.OrderBy(a => a["Name"]).ToList();
        }

        // GET
        // /api/clients/status
        [HttpGet("api/[controller]/[action]")]
        public async Task<JObject> Status()
        {
            dynamic clients = await Clients();
            var up = 0;
            var draining = 0;
            var down = 0;

            foreach (var client in clients)
            {
                if (client.Status.Value == "ready") { up++; }
                if (client.Status.Value == "down" && client.Drain.Value) { draining++; }
                if (client.Status.Value == "down" && !client.Drain.Value) { down++; }
            }

            dynamic status = new JObject();
            status.Up = up;
            status.Draining = draining;
            status.Down = down;

            return status;
        }
    }
}
