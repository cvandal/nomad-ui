using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nomad.Controllers
{
    public class ClientController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/clients")]
        public async Task<IActionResult> Clients()
        {
            var clients = await GetClientsAsync();

            return View("~/Views/Nomad/Clients.cshtml", clients);
        }

        [Route("/client")]
        public async Task<IActionResult> Client(string id)
        {
            var client = await GetClientAsync(id);
            client.Stats = await GetClientStatsAsync(client.Resources.Networks.FirstOrDefault().IP);
            client.Allocations = await GetClientAllocationsAsync(id);

            return View("~/Views/Nomad/Client.cshtml", client);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            var clients = new List<Client>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/nodes"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                clients = JsonConvert.DeserializeObject<List<Client>>(result);
            }

            foreach (var client in clients)
            {
                if (client.Status == "ready") { client.Up++; }
                if (client.Status == "down") { client.Down++; }
            }

            return clients.OrderBy(n => n.Name).ToList();
        }

        public async Task<Client> GetClientAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/node/" + id))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                return JsonConvert.DeserializeObject<Client>(result);
            }
        }

        public async Task<Stats> GetClientStatsAsync(string ip)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://" + ip + ":4646/v1/client/stats"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Stats>(result);
            }
        }

        public async Task<List<Allocation>> GetClientAllocationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/node/" + id + "/allocations"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Allocation>>(result);
            }
        }
    }
}
