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
    public class AllocationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private static HttpClient HttpClient = new HttpClient();

        [HttpGet("/allocations")]
        public async Task<IActionResult> Allocations(string search, int? page)
        {
            var allocations = await GetAllocationsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                allocations = allocations.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            ViewData["search"] = search;

            return View("~/Views/Nomad/Allocations.cshtml", PaginatedListExtension<Allocation>.CreateAsync(allocations, page ?? 1, 15));
        }

        [HttpGet("/allocation")]
        public async Task<IActionResult> Allocation(string id)
        {
            var allocation = await GetAllocationAsync(id);
            allocation.Stats = await GetAllocationStatsAsync(allocation.Resources.Networks.FirstOrDefault().IP, id);
            allocation.Logs = await GetAllocationLogsAsync(allocation.Resources.Networks.FirstOrDefault().IP, id);

            return View("~/Views/Nomad/Allocation.cshtml", allocation);
        }

        [HttpGet("/allocation/log")]
        public async Task<IActionResult> Log(string client, string id, string log)
        {
            string content = await GetAllocationLogAsync(client, id, log);

            return Content(content);
        }

        [HttpGet("/api/allocation/stats")]
        public async Task<IActionResult> Stats(string client, string id)
        {
            var stats = await GetAllocationStatsAsync(client, id);

            return Json(stats);
        }

        public async Task<List<Allocation>> GetAllocationsAsync()
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/allocations").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Allocation>>(result).OrderBy(a => a.Name).ToList();
        }

        public async Task<Allocation> GetAllocationAsync(string id)
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/allocation/" + id).Result.Content.ReadAsStringAsync();
            ViewData["Json"] = JToken.Parse(result).ToString(Formatting.Indented);

            return JsonConvert.DeserializeObject<Allocation>(result);
        }

        public List<Event> GetAllocationEvents(List<Allocation> allocations, int count)
        {
            var events = new List<Event>();

            foreach (var allocation in allocations)
            {
                if (allocation.TaskStates == null)
                    continue;

                foreach (var key in allocation.TaskStates.Keys)
                {
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "pending") { allocation.Pending++; }
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "running") { allocation.Running++; }
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "dead") { allocation.Dead++; }

                    foreach (var @event in allocation.TaskStates[key].Events)
                    {
                        @event.AllocationId = allocation.ID;
                        @event.AllocationName = allocation.Name;

                        events.Add(@event);
                    }
                }
            }

            if (count > 0)
            {
                return events.OrderByDescending(e => e.Time).Take(count).ToList();
            }

            return events.OrderByDescending(e => e.Time).ToList();
        }

        public async Task<Stats> GetAllocationStatsAsync(string client, string id)
        {
            var result = await HttpClient.GetAsync("http://" + client + ":4646/v1/client/allocation/" + id + "/stats").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Stats>(result);
        }

        public async Task<List<Log>> GetAllocationLogsAsync(string client, string id)
        {
            var result = await HttpClient.GetAsync("http://" + client + ":4646/v1/client/fs/ls/" + id + "?path=/alloc/logs").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Log>>(result);
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            return await HttpClient.GetAsync("http://" + client + ":4646/v1/client/fs/cat/" + id + "?path=/alloc/logs/" + log).Result.Content.ReadAsStringAsync();
        }
    }
}
