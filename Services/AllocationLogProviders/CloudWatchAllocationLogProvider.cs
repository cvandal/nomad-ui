using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nomad.Services.AllocationLogProviders
{
    public class CloudWatchAllocationLogProvider : IAllocationLogProvider
    {
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
            DescribeLogGroupsResponse res;
            using (var amazonCloudWatchLogsClient = new AmazonCloudWatchLogsClient())
                res = await amazonCloudWatchLogsClient.DescribeLogGroupsAsync(new DescribeLogGroupsRequest());
            
            foreach (var logGroup in res.LogGroups)
            {
                Console.WriteLine(logGroup.LogGroupName);
            }

            return JsonConvert.DeserializeObject<JArray>(res.LogGroups.ToString());
        }

        public async Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            return "Hello, world!";
        }
    }
}
