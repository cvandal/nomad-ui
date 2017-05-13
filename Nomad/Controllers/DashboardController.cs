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
            var clientsTask = new ClientController().GetClientsAsync();
            var serverTask = new ServerController().GetServersAsync();

            await Task.WhenAll(jobsTask, allocationsTask, clientsTask, serverTask);

            var dashboard = new Dashboard
            {
                Jobs = await jobsTask,
                Allocations = await allocationsTask,
                Clients = await clientsTask,
                Server = await serverTask,
                Events = new AllocationController().GetAllocationEvents(await allocationsTask)
            };

            return View("~/Views/Nomad/Dashboard.cshtml", dashboard);
        }
    }
}
