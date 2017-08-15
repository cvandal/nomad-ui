using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nomad.Models;

namespace Nomad.Services.AllocationLogProviders
{
    public class NomadAllocationLogProvider : IAllocationLogProvider
    {
        private readonly int _port;
        private readonly HttpClient _httpClient = new HttpClient();

        public NomadAllocationLogProvider():this(4646)
        {   
        }

        public NomadAllocationLogProvider(int port=4646)
        {
            _port = port;
        }



        public async Task<bool> CanProvideAsync(string client)
        {
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            using (var response =
                await _httpClient.GetAsync($"http://{client}:{_port}/v1/client/fs/cat/{id}?path=/alloc/logs/{log}"))
            {
                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }

        public void AssignClientsAsync(IList<string> clients)
        {
            // do nothing
        }

        public async Task<List<Log>> GetAllocationLogsAsync(string client, string id)
        {
            using (var response =
                await _httpClient.GetAsync($"http://{client}:{_port}/v1/client/fs/ls/{id}?path=/alloc/logs"))
            {
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Log>>(result);
                }
            }
        }
    }
}