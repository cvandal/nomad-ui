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
using Nomad.Services.AllocationLogProviders;

namespace Nomad.Controllers
{
    public class AllocationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private readonly IAllocationLogProviderFactory _allocationLogProviderFactory;

        public AllocationController(IAllocationLogProviderFactory allocationLogProviderFactory)
        {
            _allocationLogProviderFactory = allocationLogProviderFactory;
        }
        
        // GET /allocation
        [HttpGet("[action]")]
        public IActionResult Allocation()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/allocation?id={id}
        [HttpGet("api/allocation")]
        public async Task<JObject> GetAllocationAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/allocation/" + id))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic allocation = JsonConvert.DeserializeObject<JObject>(await content);
                allocation.CreateTime = DateTimeExtension.FromUnixTime(Convert.ToInt64(allocation.CreateTime.Value)); // Convert the allocation create time from UnixTime (milliseconds) to DateTime
                allocation.Stats = await GetAllocationStatsAsync(allocation.Resources.Networks[0].IP.Value, id);
                allocation.Logs = await GetAllocationLogsAsync(allocation.Resources.Networks[0].IP.Value, id);

                if (allocation.TaskStates != null)
                {
                    foreach (var key in allocation.TaskStates)
                    {
                        foreach (var @event in allocation.TaskStates[key.Name].Events)
                        {
                            @event.Time = DateTimeExtension.FromUnixTime(Convert.ToInt64(@event.Time.Value)); // Convert the allocation time from UnixTime (milliseconds) to DateTime
                        }
                    }
                }

                return allocation;
            }
        }

        public async Task<JObject> GetAllocationStatsAsync(string client, string id)
        {
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync("http://" + client + ":4646/v1/client/allocation/" + id + "/stats"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                return JsonConvert.DeserializeObject<JObject>(await content);
            }
        }

        public async Task<JArray> GetAllocationLogsAsync(string client, string id)
        {
            var allocationLogProvider = await _allocationLogProviderFactory.GetAllocationLogProviderAsync(client).ConfigureAwait(false);
            return await allocationLogProvider.GetAllocationLogsAsync(client, id).ConfigureAwait(false);
        }

        // GET /api/allocation/log?client={client}&id={id}&log={log}
        [HttpGet("api/allocation/log")]
        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            var allocationLogProvider = await _allocationLogProviderFactory.GetAllocationLogProviderAsync(client).ConfigureAwait(false);
            return await allocationLogProvider.GetAllocationLogAsync(client, id, log).ConfigureAwait(false);
        }

        // GET /allocations
        [HttpGet("[action]")]
        public IActionResult Allocations()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/allocations
        [HttpGet("api/allocations")]
        public async Task<List<JObject>> GetAllocationsAsync(string search = null)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/allocations"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic json = JsonConvert.DeserializeObject<List<JObject>>(await content);
                var allocations = new List<JObject>();
                
                foreach (var allocation in json)
                {
                    allocation.CreateTime = DateTimeExtension.FromUnixTime(Convert.ToInt64(allocation.CreateTime.Value)); // Convert the allocation create time from UnixTime (milliseconds) to DateTime
                    allocations.Add(allocation);
                }

                if (!String.IsNullOrEmpty(search))
                    allocations = allocations.Where(a => a["Name"].ToString().ToLower().Contains(search.ToLower())).ToList();

                return allocations.OrderBy(a => a["Name"]).ToList();
            }
        }

        // GET /api/allocations/status
        [HttpGet("api/allocations/status")]
        public async Task<JObject> GetAllocationsStatusAsync()
        {
            dynamic allocations = await GetAllocationsAsync();
            var running = 0;
            var pending = 0;
            var dead = 0;

            foreach (var allocation in allocations)
            {
                if (allocation.TaskStates == null)
                    continue;

                foreach (var key in allocation.TaskStates)
                {
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key.Name].State == "running") { running++; }
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key.Name].State == "pending") { pending++; }
                    if (allocation.DesiredStatus == "run" && allocation.TaskStates[key.Name].State == "dead") { dead++; }
                }
            }

            dynamic status = new JObject();
            status.Running = running;
            status.Pending = pending;
            status.Dead = dead;

            return status;
        }

        // GET /api/allocations/events
        [HttpGet("api/allocations/events")]
        public async Task<List<JObject>> GetAllocationsEventsAsync(int count)
        {
            dynamic allocations = await GetAllocationsAsync();
            var events = new List<JObject>();

            foreach (var allocation in allocations)
            {
                if (allocation.TaskStates == null)
                    continue;

                foreach (var key in allocation.TaskStates)
                {
                    foreach (var @event in allocation.TaskStates[key.Name].Events)
                    {
                        @event.ID = allocation.ID; // Add the corresponding allocation ID to the allocation event
                        @event.Name = allocation.Name; // Add the corresponding allocation name to the allocation event
                        @event.Time = DateTimeExtension.FromUnixTime(Convert.ToInt64(@event.Time.Value)); // Convert the allocation time from UnixTime (milliseconds) to DateTime
                        events.Add(@event);
                    }
                }
            }

            if (count > 0)
                return events.OrderByDescending(e => e["Time"]).Take(count).ToList();

            return events.OrderByDescending(e => e["Time"]).ToList();
        }
    }
}
