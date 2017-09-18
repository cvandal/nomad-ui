using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nomad.Extensions;

namespace Nomad.Controllers
{
    public class JobController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        // GET /job
        [HttpGet("[action]")]
        public IActionResult Job()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/job?id={id}
        [HttpGet("api/job")]
        public async Task<JObject> GetJobAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/job/" + id))
            using (var content = response.Content.ReadAsStringAsync())
            {
                return JsonConvert.DeserializeObject<JObject>(await content);
            }
        }

        // GET /api/job/evaluations?id={id}
        [HttpGet("api/job/evaluations")]
        public async Task<List<JObject>> GetJobEvaluationsAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/job/" + id + "/evaluations"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                return JsonConvert.DeserializeObject<List<JObject>>(await content);
            }
        }

        // GET /api/job/allocations?id={id}
        [HttpGet("api/job/allocations")]
        public async Task<List<JObject>> GetJobAllocationsAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/job/" + id + "/allocations"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic json = JsonConvert.DeserializeObject<List<JObject>>(await content);
                var allocations = new List<JObject>();

                foreach (var allocation in json)
                {
                    allocation.CreateTime = DateTimeExtension.FromUnixTime(Convert.ToInt64(allocation.CreateTime.Value)); // Convert the job allocation create time from UnixTime (milliseconds) to DateTime
                    allocations.Add(allocation);
                }

                return allocations;
            }
        }

        // GET /jobs
        [HttpGet("[action]")]
        public IActionResult Jobs()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/jobs
        [HttpGet("api/jobs")]
        public async Task<List<JObject>> GetJobsAsync(string search = null)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/jobs"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic json = JsonConvert.DeserializeObject<List<JObject>>(await content);
                var jobs = new List<JObject>();

                foreach (var job in json)
                {
                    var totalTaskGroups = 0;
                    var totalQueuedTasks = 0;
                    var totalStartingTasks = 0;
                    var totalRunningTasks = 0;
                    var totalFailedTasks = 0;
                    var totalLostTasks = 0;
                    var totalCompleteTasks = 0;

                    foreach (var key in job.JobSummary.Summary)
                    {
                        totalTaskGroups++;
                        job.TotalQueuedTasks = totalQueuedTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Queued.Value);
                        job.TotalStartingTasks = totalStartingTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Starting.Value);
                        job.TotalRunningTasks = totalRunningTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Running.Value);
                        job.TotalFailedTasks = totalFailedTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Failed.Value);
                        job.TotalLostTasks = totalLostTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Lost.Value);
                        job.TotalCompleteTasks = totalCompleteTasks + Convert.ToInt32(job.JobSummary.Summary[key.Name].Complete.Value);
                    }

                    job.TotalTaskGroups = totalTaskGroups;
                    job.SubmitTime = DateTimeExtension.FromUnixTime(Convert.ToInt64(job.SubmitTime.Value)); // Convert the job submit time from UnixTime (milliseconds) to DateTime
                    jobs.Add(job);
                }

                if (!String.IsNullOrEmpty(search))
                    jobs = jobs.Where(j => j["ID"].ToString().ToLower().Contains(search.ToLower())).ToList();

                return jobs.OrderBy(j => j["ID"]).ToList();
            }
        }

        // GET /api/jobs/status
        [HttpGet("api/jobs/status")]
        public async Task<JObject> GetJobsStatusAsync()
        {
            dynamic jobs = await GetJobsAsync();
            var running = 0;
            var pending = 0;
            var dead = 0;

            foreach (var job in jobs)
            {
                if (job.Status == "running") { running++; }
                if (job.Status == "pending") { pending++; }
                if (job.Status == "dead") { dead++; }
            }

            dynamic status = new JObject();
            status.Running = running;
            status.Pending = pending;
            status.Dead = dead;

            return status;
        }
    }
}
