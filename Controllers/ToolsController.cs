using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class ToolsController : Controller
    {
        private static readonly string NomadUrl = Environment.GetEnvironmentVariable("NOMAD_URL");

        [HttpGet("/tools/gc")]
        public async Task<HttpResponseMessage> GarbageCollection()
        {
            var httpContent = new StringContent("", Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PutAsync(NomadUrl + "/v1/system/gc", httpContent))
            {
                return response;
            }
        }
    }
}
