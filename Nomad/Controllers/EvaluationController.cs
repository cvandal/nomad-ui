using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nomad.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nomad.Extensions;

namespace Nomad.Controllers
{
    public class EvaluationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        private static HttpClient HttpClient = new HttpClient();

        [Route("/evaluations")]
        public async Task<IActionResult> Evaluations(string search, int? page)
        {
            var evaluations = await GetEvaluationsAsync();

            if (!String.IsNullOrEmpty(search))
            {
                evaluations = evaluations.Where(e => e.JobID.ToLower().Contains(search.ToLower())).ToList();
            }

            return View("~/Views/Nomad/Evaluations.cshtml", PaginatedList<Evaluation>.CreateAsync(evaluations, page ?? 1, 15));
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
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/evaluations").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Evaluation>>(result).OrderBy(e => e.JobID).ToList();
        }

        public async Task<Evaluation> GetEvaluationAsync(string id)
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/evaluation/" + id).Result.Content.ReadAsStringAsync();
            ViewData["Json"] = JToken.Parse(result).ToString(Formatting.Indented);

            return JsonConvert.DeserializeObject<Evaluation>(result);
        }

        public async Task<List<Allocation>> GetEvaluationAllocationsAsync(string id)
        {
            var result = await HttpClient.GetAsync(NomadUrl + "/v1/evaluation/" + id + "/allocations").Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Allocation>>(result).OrderBy(a => a.Name).ToList();
        }
    }
}
