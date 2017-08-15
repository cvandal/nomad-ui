using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System;
using Nomad.Services.AllocationLogProviders;

namespace Nomad.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAllocationLogProviderFactory _allocationLogProviderFactory;

        public DashboardController(IAllocationLogProviderFactory allocationLogProviderFactory)
        {
            _allocationLogProviderFactory = allocationLogProviderFactory;
        }

        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/dashboard")]
        public async Task<JsonResult> GetDashboardAsJsonResult()
        {
            var jobs = new JobController().GetJobsAsync();
            var allocations = new AllocationController(_allocationLogProviderFactory).GetAllocationsAsync();
            var clients = new ClientController().GetClientsAsync();
            var servers = new ServerController().GetServersAsync();
            await System.Threading.Tasks.Task.WhenAll(jobs, allocations, clients, servers);

            var dashboard = new Dashboard
            {
                Jobs = await jobs,
                Allocations = await allocations,
                Clients = await clients,
                Servers = await servers,
                Events = new AllocationController(_allocationLogProviderFactory).GetAllocationEvents(await allocations, 15)
            };

            return Json(dashboard);
        }
    }
}
