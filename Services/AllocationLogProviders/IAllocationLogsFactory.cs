using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public interface IAllocationLogProviderFactory
    {
        Task<IAllocationLogProvider> GetAllocationLogProviderAsync(string client);
    }
}
