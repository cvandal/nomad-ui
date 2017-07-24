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
    public class EvaluationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [HttpGet("/evaluations")]
        public IActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/evaluations")]
        public async Task<JsonResult> GetEvaluationsAsJsonResult(string search)
        {
            var evaluations = await GetEvaluationsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                evaluations = evaluations.Where(e => e.JobID.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(evaluations);
        }

        [HttpGet("/evaluation")]
        public IActionResult Evaluation(string id)
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpGet("/api/evaluation")]
        public async Task<IActionResult> GetEvaluationAsJsonResult(string id)
        {
            var evaluation = await GetEvaluationAsync(id);
            evaluation.Allocations = await GetEvaluationAllocationsAsync(id);

            return Json(evaluation);
        }

        public async Task<List<Evaluation>> GetEvaluationsAsync()
        {
            List<Evaluation> evaluations;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                evaluations = JsonConvert.DeserializeObject<List<Evaluation>>(result);
            }

            return evaluations.OrderBy(e => e.JobID).ToList();
        }

        public async Task<Evaluation> GetEvaluationAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Evaluation>(result);
            }
        }

        public async Task<List<Allocation>> GetEvaluationAllocationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id + "/allocations"))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Allocation>>(result);
            }
        }
    }
}
