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
    public class ClientController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [HttpGet("/clients")]
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/clients")]
        public async Task<JsonResult> GetClientsAsJsonResult(string search)
        {
            var clients = await GetClientsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                clients = clients.Where(c => c.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(clients);
        }

        [HttpGet("/client")]
        public IActionResult Client()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/client")]
        public async Task<JsonResult> GetClientAsJsonResult(string id)
        {
            var client = await GetClientAsync(id);
            client.Stats = await GetClientStatsAsync(client.Resources.Networks.FirstOrDefault().IP);
            client.Allocations = await GetClientAllocationsAsync(id);

            return Json(client);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            List<Client> clients;

            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(NomadUrl + "/v1/nodes"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                clients = JsonConvert.DeserializeObject<List<Client>>(result);

                foreach (var client in clients)
                {
                    if (client.Status == "ready") { client.Up++; }
                    if (client.Status == "down" && !client.Drain) { client.Down++; }
                    if (client.Status == "down" && client.Drain) { client.Draining++; }
                }
            }

            return clients.OrderBy(c => c.Name).ToList();
        }

        public async Task<Client> GetClientAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/node/" + id))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Client>(result);
            }
        }

        public async Task<Stats> GetClientStatsAsync(string ip)
        {
            var address = ip.ConvertToFriendlyAddress();

            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(address + "/v1/client/stats"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Stats>(result);
            }
        }

        public async Task<List<Allocation>> GetClientAllocationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/node/" + id + "/allocations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Allocation>>(result);
            }
        }
    }
}
