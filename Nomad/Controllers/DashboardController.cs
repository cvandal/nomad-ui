using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;

namespace Nomad.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var dashboard = new Dashboard();
            dashboard.Jobs = await new JobController().GetJobsAsync();
            dashboard.Allocations = await new AllocationController().GetAllocationsAsync();
            dashboard.Events = new AllocationController().GetAllocationEvents(dashboard.Allocations);
            dashboard.Nodes = await new NodeController().GetNodesAsync();
            dashboard.Agent = await new AgentController().GetAgentsAsync();
            
            return View("~/Views/Nomad/Dashboard.cshtml", dashboard);
        }
    }
}
