using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Nomad.Models;
using Task = System.Threading.Tasks.Task;

namespace Nomad.Services.AllocationLogProviders
{
    public class InMemoryAllocationLogProvider : IAllocationLogProvider
    {
        private IList<string> _clients;

        public InMemoryAllocationLogProvider()
        {
            _clients = new List<string>();
        }

        public async Task AssignClientsAsync(IList<string> clients)
        {
            _clients = clients ?? new List<string>();
            await Task.FromResult<object>(null);
        }

        public Task<bool> CanProvideAsync(string client)
        {
            return Task.FromResult<bool>(_clients.Contains(client));
        }

        public Task<string> GetAllocationLogAsync(string client, string id, string log)
        {
            return Task.FromResult("Fun log");
        }

        public Task<List<Log>> GetAllocationLogsAsync(string client, string id)
        {
            return Task.FromResult(new List<Log>()
            {
                new Log() {FileMode = "open", IsDir = false, ModTime = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture),
                    Name = "FileLog", Size = 1234543}
            });
        }
    }
}