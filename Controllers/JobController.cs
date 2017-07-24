using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nomad.Models;

namespace Nomad.Controllers
{
    public class JobController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [HttpGet("/jobs")]
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/jobs")]
        public async Task<JsonResult> GetJobsAsJsonResult(string search)
        {
            var jobs = await GetJobsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                jobs = jobs.Where(j => j.ID.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(jobs);
        }

        [HttpGet("/job")]
        public IActionResult Job()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/job")]
        public async Task<JsonResult> GetJobAsJsonResult(string id)
        {
            var job = await GetJobAsync(id);
            job.Evaluations = await GetJobEvaluationsAsync(id);
            job.Allocations = await GetJobAllocationsAsync(id);

            return Json(job);
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            List<Job> jobs;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/jobs"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                jobs = JsonConvert.DeserializeObject<List<Job>>(result);

                foreach (var job in jobs)
                {
                    if (job.Status == "running") { job.Running++; }
                    if (job.Status == "pending") { job.Pending++; }
                    if (job.Status == "dead") { job.Dead++; }

                    job.NumOfTaskGroups = job.JobSummary.Summary.Keys.Count();

                    foreach (var value in job.JobSummary.Summary.Values)
                    {
                        job.QueuedTaskGroups = job.QueuedTaskGroups + value.Queued;
                        job.CompleteTaskGroups = job.CompleteTaskGroups + value.Complete;
                        job.FailedTaskGroups = job.FailedTaskGroups + value.Failed;
                        job.RunningTaskGroups = job.RunningTaskGroups + value.Running;
                        job.StartingTaskGroups = job.StartingTaskGroups + value.Starting;
                        job.LostTaskGroups = job.LostTaskGroups + value.Lost;
                    }
                }
            }

            return jobs.OrderBy(j => j.Name).ToList();
        }

        public async Task<Job> GetJobAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/job/" + id))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Job>(result);
            }
        }

        public async Task<List<Evaluation>> GetJobEvaluationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/job/" + id + "/evaluations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Evaluation>>(result);
            }
        }

        public async Task<List<Allocation>> GetJobAllocationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/job/" + id + "/allocations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Allocation>>(result);
            }
        }
    }
}
