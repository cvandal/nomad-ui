using System.Collections.Generic;
using System.Threading.Tasks;
using Nomad.Models;

namespace Nomad.Services.AllocationLogProviders
{
    public interface IAllocationLogProvider
    {
        Task<bool> CanProvideAsync(string client);
        Task<List<Log>> GetAllocationLogsAsync(string client, string id);
        Task<string> GetAllocationLogAsync(string client, string id, string log);
        System.Threading.Tasks.Task AssignClientsAsync(IList<string> clients);
    }
}