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
    public class NodeController : Controller
    {
        public static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/clients")]
        public async Task<IActionResult> Clients()
        {
            var clients = await GetNodesAsync();

            return View("~/Views/Nomad/Clients.cshtml", clients);
        }

        [Route("/client")]
        public async Task<IActionResult> Client(string id)
        {
            var client = await GetNodeAsync(id);
            client.Stats = await GetNodeStatsAsync(client.Resources.Networks.FirstOrDefault().IP);
            client.Allocations = await GetNodeAllocationsAsync(id);

            return View("~/Views/Nomad/Client.cshtml", client);
        }

        public async Task<List<Node>> GetNodesAsync()
        {
            var nodes = new List<Node>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/nodes"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                nodes = JsonConvert.DeserializeObject<List<Node>>(result);
            }

            foreach (var node in nodes)
            {
                if (node.Status == "ready") { node.Up++; }
                if (node.Status == "down") { node.Down++; }
            }

            return nodes.OrderBy(n => n.Name).ToList();
        }

        public async Task<Node> GetNodeAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/node/" + id))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                return JsonConvert.DeserializeObject<Node>(result);
            }
        }

        public async Task<Stats> GetNodeStatsAsync(string ip)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://" + ip + ":4646/v1/client/stats"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Stats>(result);
            }
        }

        public async Task<List<Allocation>> GetNodeAllocationsAsync(string id)
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
