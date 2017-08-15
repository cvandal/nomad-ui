using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nomad.Models;
using Nomad.Services.AllocationLogProviders;

namespace Nomad.Controllers
{
    public class AllocationController : Controller
    {
        private readonly IAllocationLogProviderFactory _allocationLogProviderFactory;
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        public AllocationController(IAllocationLogProviderFactory allocationLogProviderFactory)
        {
            _allocationLogProviderFactory = allocationLogProviderFactory;
        }

        [HttpGet("/allocations")]
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/allocations")]
        public async Task<IActionResult> GetAllocationsAsJsonResult(string search)
        {
            var allocations = await GetAllocationsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                allocations = allocations.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(allocations);
        }

        [HttpGet("/allocation")]
        public IActionResult Allocation()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/allocation")]
        public async Task<IActionResult> GetAllocationAsJsonResult(string id)
        {
            var allocation = await GetAllocationAsync(id);
            allocation.Stats = await GetAllocationStatsAsync(allocation.Resources.Networks.FirstOrDefault().IP, id);
            allocation.Logs = await GetAllocationLogsAsync(allocation.Resources.Networks.FirstOrDefault().IP, id);

            return Json(allocation);
        }

        [HttpGet("/allocation/log")]
        public async Task<IActionResult> Log(string client, string id, string log)
        {
            var content = await GetAllocationLogAsync(client, id, log);

            return Content(content);
        }

        public async Task<List<Allocation>> GetAllocationsAsync()
        {
            List<Allocation> allocations;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/allocations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                allocations = JsonConvert.DeserializeObject<List<Allocation>>(result);

                foreach (var allocation in allocations)
                {
                    if (allocation.TaskStates == null)
                        continue;
                    
                    foreach (var key in allocation.TaskStates.Keys)
                    {
                        if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "running") { allocation.Running++; }
                        if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "pending") { allocation.Pending++; }
                        if (allocation.DesiredStatus == "run" && allocation.TaskStates[key].State == "dead") { allocation.Dead++; }
                    }
                }
            }

            return allocations.OrderBy(a => a.Name).ToList();
        }

        public async Task<Allocation> GetAllocationAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/allocation/" + id))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Allocation>(result);
            }
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
                    foreach (var @event in allocation.TaskStates[key].Events)
                    {
                        @event.AllocationID = allocation.ID;
                        @event.AllocationName = allocation.Name;
                        events.Add(@event);
                    }
                }
            }

            if (count > 0)
                return events.OrderByDescending(e => e.DateTime).Take(15).ToList();

            return events.OrderByDescending(e => e.DateTime).ToList();
        }

        public async Task<Stats> GetAllocationStatsAsync(string client, string id)
        {
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync("http://" + client + ":4646/v1/client/allocation/" + id + "/stats"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Stats>(result);
            }
        }

        public async Task<List<Log>> GetAllocationLogsAsync(string client, string id)
        {
            var allocationLogProvider = await _allocationLogProviderFactory.GetAllocationLogProviderAsync(client).ConfigureAwait(false);
            return await allocationLogProvider.GetAllocationLogsAsync(client, id).ConfigureAwait(false);
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            var allocationLogProvider = await _allocationLogProviderFactory.GetAllocationLogProviderAsync(client).ConfigureAwait(false);
            return await allocationLogProvider.GetAllocationLogAsync(client, id, log).ConfigureAwait(false);
        }
    }
}
