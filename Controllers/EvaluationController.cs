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
    public class EvaluationController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");
        
        // GET /evaluation
        [HttpGet("[action]")]
        public IActionResult Evaluation()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/evaluation?id={id}
        [HttpGet("api/evaluation")]
        public async Task<JObject> GetEvaluationAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id))
            using (var content = response.Content.ReadAsStringAsync())
            {
                return JsonConvert.DeserializeObject<JObject>(await content);
            }
        }

        // GET /api/evaluation/allocations?id={id}
        [HttpGet("api/evaluation/allocations")]
        public async Task<List<JObject>> GetEvaluationAllocationsAsync(string id)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/evaluation/" + id + "/allocations"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic json = JsonConvert.DeserializeObject<List<JObject>>(await content);
                var allocations = new List<JObject>();

                foreach (var allocation in json)
                {
                    allocation.CreateTime = DateTimeExtension.FromUnixTime(Convert.ToInt64(allocation.CreateTime.Value)); // Convert the evaluation allocation create time from UnixTime (milliseconds) to DateTime
                    allocations.Add(allocation);
                }

                return allocations;
            }
        }

        // GET /evaluations
        [HttpGet("[action]")]
        public IActionResult Evaluations()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }

        // GET /api/evaluations
        [HttpGet("api/evaluations")]
        public async Task<List<JObject>> GetEvaluationsAsync(string search = null)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(NomadUrl + "/v1/evaluations"))
            using (var content = response.Content.ReadAsStringAsync())
            {
                dynamic json = JsonConvert.DeserializeObject<List<JObject>>(await content);
                var evaluations = new List<JObject>();

                foreach (var evaluation in json)
                    evaluations.Add(evaluation);

                if (!String.IsNullOrEmpty(search))
                    evaluations = evaluations.Where(e => e["JobID"].ToString().ToLower().Contains(search.ToLower())).ToList();

                return evaluations.OrderBy(e => e["JobID"]).ToList();
            }
        }
    }
}
