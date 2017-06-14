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
    public class ClientController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private static HttpClient HttpClient = new HttpClient();

        [HttpGet("/clients")]
        public async Task<IActionResult> Clients(string search, int? page)
        {
            var clients = await GetClientsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                clients = clients.Where(c => c.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            ViewData["search"] = search;

            return View("~/Views/Nomad/Clients.cshtml", PaginatedListExtension<Client>.CreateAsync(clients, page ?? 1, 15));
        }

        [HttpGet("/client")]
        public async Task<IActionResult> Client(string id)
        {
            var client = await GetClientAsync(id);
            client.Stats = await GetClientStatsAsync(client.Resources.Networks.FirstOrDefault().IP);
            client.Allocations = await GetClientAllocationsAsync(id);

            return View("~/Views/Nomad/Client.cshtml", client);
        }

        [HttpGet("/api/client/stats")]
        public async Task<IActionResult> Stats(string client)
        {
            var stats = await GetClientStatsAsync(client);

            return Json(stats);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            List<Client> clients;

            var result = await HttpClient.GetAsync(NomadUrl + "/v1/nodes").Result.Content.ReadAsStringAsync();

            clients = JsonConvert.DeserializeObject<List<Client>>(result);

            foreach (var client in clients)
            {
                if (client.Status == "ready") { client.Up++; }
                if (client.Status == "down" && !client.Drain) { client.Down++; }
                if (client.Status == "down" && client.Drain) { client.Draining++; }
            }

            return clients.OrderBy(c => c.Name).ToList();
        }

        public async Task<Client> GetClientAsync(string id)
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/node/" + id).Result.Content.ReadAsStringAsync();
            ViewData["Json"] = JToken.Parse(result).ToString(Formatting.Indented);

            return JsonConvert.DeserializeObject<Client>(result);
        }

        public async Task<Stats> GetClientStatsAsync(string client)
        {
            var result = await HttpClient.GetAsync("http://" + client + ":4646/v1/client/stats").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Stats>(result);
        }

        public async Task<List<Allocation>> GetClientAllocationsAsync(string id)
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/node/" + id + "/allocations").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Allocation>>(result);
        }
    }
}
