using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nomad.Services.AllocationLogProviders
{
    public class DefaultAllocationLogProvider : IAllocationLogProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<bool> CanProvideAsync(string client)
        {
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        public async System.Threading.Tasks.Task AssignClientsAsync(IList<string> clients)
        {
            await System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public async Task<JArray> GetAllocationLogsAsync(string client, string id)
        {
            using (var response = await _httpClient.GetAsync($"http://{client}:4646/v1/client/fs/ls/{id}?path=/alloc/logs"))
            {
                using (var content = response.Content.ReadAsStringAsync())
                {
                    return JsonConvert.DeserializeObject<JArray>(await content);
                }
            }
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            using (var response = await _httpClient.GetAsync($"http://{client}:4646/v1/client/fs/cat/{id}?path=/alloc/logs/{log}"))
            {
                using (var content = response.Content.ReadAsStringAsync())
                {
                    return await content;
                }
            }
        }
    }
}
