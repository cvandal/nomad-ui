using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using Task = System.Threading.Tasks.Task;

namespace Nomad.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await GetDashboardAsync();

            return View("~/Views/Nomad/Dashboard.cshtml", dashboard);
        }

        [Route("/api/dashboard")]
        public async Task<IActionResult> DashboardApi()
        {
            var dashboard = await GetDashboardAsync();

            return Json(dashboard);
        }

        public async Task<Dashboard> GetDashboardAsync()
        {
            var jobsTask = new JobController().GetJobsAsync();
            var allocationsTask = new AllocationController().GetAllocationsAsync();
            var clientsTask = new ClientController().GetClientsAsync();
            var serverTask = new ServerController().GetServersAsync();

            await Task.WhenAll(jobsTask, allocationsTask, clientsTask, serverTask);

            var dashboard = new Dashboard
            {
                Jobs = await jobsTask,
                Allocations = await allocationsTask,
                Clients = await clientsTask,
                Servers = await serverTask,
                Events = new AllocationController().GetAllocationEvents(await allocationsTask)
            };

            return dashboard;
        }
    }
}
