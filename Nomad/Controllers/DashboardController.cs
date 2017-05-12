using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using Task = System.Threading.Tasks.Task;

namespace Nomad.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var jobsTask = new JobController().GetJobsAsync();
            var allocationsTask = new AllocationController().GetAllocationsAsync();
            var nodesTask = new NodeController().GetNodesAsync();
            var agentTask = new AgentController().GetAgentsAsync();

            await Task.WhenAll(jobsTask, allocationsTask, nodesTask, agentTask);

            var dashboard = new Dashboard
            {
                Jobs = await jobsTask,
                Allocations = await allocationsTask,
                Nodes = await nodesTask,
                Agent = await agentTask,
                Events = new AllocationController().GetAllocationEvents(await allocationsTask)
            };

            return View("~/Views/Nomad/Dashboard.cshtml", dashboard);
        }
    }
}
