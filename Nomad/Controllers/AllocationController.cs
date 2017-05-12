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
    public class AllocationController : Controller
    {
        public static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/allocations")]
        public async Task<IActionResult> Allocations()
        {
            var allocations = await GetAllocationsAsync();

            return View("~/Views/Nomad/Allocations.cshtml", allocations);
        }

        [Route("/allocation")]
        public async Task<IActionResult> Allocation(string id)
        {
            var allocation = await GetAllocationAsync(id);
            allocation.Stats = await GetAllocationStatsAsync(allocation.Resources.Networks.FirstOrDefault().IP, allocation.ID);
            allocation.Logs = await GetAllocationLogsAsync(allocation.Resources.Networks.FirstOrDefault().IP, allocation.ID);

            return View("~/Views/Nomad/Allocation.cshtml", allocation);
        }

        [Route("/allocation/log")]
        public async Task<IActionResult> Log(string client, string id, string log)
        {
            string content = await GetAllocationLogAsync(client, id, log);

            return Content(content);
        }

        public async Task<List<Allocation>> GetAllocationsAsync()
        {
            var allocations = new List<Allocation>();
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/allocations"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                allocations = JsonConvert.DeserializeObject<List<Allocation>>(result);
            }

            foreach (var allocation in allocations)
            {
                allocation.CreateTime = dateTime.AddTicks(Convert.ToInt64(allocation.CreateTime) / (TimeSpan.TicksPerMillisecond / 100)).ToLocalTime();
            }

            return allocations.OrderBy(a => a.Name).ToList();
        }

        public async Task<Allocation> GetAllocationAsync(string id)
        {
            var allocation = new Allocation();
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/allocation/" + id))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                allocation = JsonConvert.DeserializeObject<Allocation>(result);
            }

            allocation.CreateTime = dateTime.AddTicks(Convert.ToInt64(allocation.CreateTime) / (TimeSpan.TicksPerMillisecond / 100)).ToLocalTime();

            return allocation;
        }

        public List<Event> GetAllocationEvents(List<Allocation> allocations)
        {
            var events = new List<Event>();
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

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
                        @event.Time = dateTime.AddTicks(Convert.ToInt64(@event.Time) / (TimeSpan.TicksPerMillisecond / 100)).ToLocalTime();

                        events.Add(@event);
                    }
                }
            }

            return events.OrderByDescending(e => e.Time).ToList();
        }

        public async Task<Stats> GetAllocationStatsAsync(string client, string id)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync("http://" + client + ":4646/v1/client/allocation/" + id + "/stats"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Stats>(result);
            }
        }

        public async Task<List<Log>> GetAllocationLogsAsync(string client, string id)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync("http://" + client + ":4646/v1/client/fs/ls/" + id + "?path=/alloc/logs"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Log>>(result);
            }
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync("http://" + client + ":4646/v1/client/fs/cat/" + id + "?path=/alloc/logs/" + log))
            using (HttpContent content = response.Content)
            {
                return await content.ReadAsStringAsync();
            }
        }
    }
}
