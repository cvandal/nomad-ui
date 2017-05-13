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
    public class JobController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private static HttpClient Client = new HttpClient();

        [Route("/jobs")]
        public async Task<IActionResult> Jobs()
        {
            var jobs = await GetJobsAsync();

            return View("~/Views/Nomad/Jobs.cshtml", jobs);
        }

        [Route("/job")]
        public async Task<IActionResult> Job(string id)
        {
            var jobTask = GetJobAsync(id);
            var jobEvaluationsTask = GetJobEvaluationsAsync(id);
            var jobAllocationsTask = GetJobAllocationsAsync(id);

            var job = await jobTask;
            job.Evaluations = await jobEvaluationsTask;
            job.Allocations = await jobAllocationsTask;

            return View("~/Views/Nomad/Job.cshtml", job);
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            List<Job> jobs;

            var result = await Client.GetAsync(NomadUrl + "/v1/jobs").Result.Content.ReadAsStringAsync();
            
            jobs = JsonConvert.DeserializeObject<List<Job>>(result);

            foreach (var job in jobs)
            {
                if (job.Status == "pending") { job.Pending++; }
                if (job.Status == "running") { job.Running++; }
                if (job.Status == "dead") { job.Dead++; }
            }

            return jobs.OrderBy(j => j.ID).ToList();
        }

        public async Task<Job> GetJobAsync(string id)
        {
            var result = await Client.GetAsync(NomadUrl + "/v1/job/" + id).Result.Content.ReadAsStringAsync();

            ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

            return JsonConvert.DeserializeObject<Job>(result);
        }

        public async Task<List<Evaluation>> GetJobEvaluationsAsync(string id)
        {
            var result = await Client.GetAsync(NomadUrl + "/v1/job/" + id + "/evaluations").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Evaluation>>(result).OrderBy(e => e.JobID).ToList();
        }

        public async Task<List<Allocation>> GetJobAllocationsAsync(string id)
        {
            var result = await Client.GetAsync(NomadUrl + "/v1/job/" + id + "/allocations").Result.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<List<Allocation>>(result).OrderBy(a => a.Name).ToList();
        }
    }
}
