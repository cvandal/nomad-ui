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
    public class EvaluationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [Route("/evaluations")]
        public async Task<IActionResult> Evaluations()
        {
            var evaluations = await GetEvaluationsAsync();

            return View("~/Views/Nomad/Evaluations.cshtml", evaluations);
        }

        [Route("/evaluation")]
        public async Task<IActionResult> Evaluation(string id)
        {
            var evaluation = await GetEvaluationAsync(id);
            evaluation.Allocations = await GetEvaluationAllocationsAsync(id);

            return View("~/Views/Nomad/Evaluation.cshtml", evaluation);
        }

        public async Task<List<Evaluation>> GetEvaluationsAsync()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluations"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Evaluation>>(result).OrderBy(e => e.JobID).ToList();
            }
        }

        public async Task<Evaluation> GetEvaluationAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                ViewBag.Json = JToken.Parse(result).ToString(Formatting.Indented);

                return JsonConvert.DeserializeObject<Evaluation>(result);
            }
        }

        public async Task<List<Allocation>> GetEvaluationAllocationsAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id + "/allocations"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                var allocations = JsonConvert.DeserializeObject<List<Allocation>>(result);

                return allocations.OrderBy(a => a.Name).ToList();
            }
        }
    }
}
