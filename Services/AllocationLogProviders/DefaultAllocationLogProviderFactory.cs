using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public class DefaultAllocationLogProviderFactory : IAllocationLogProviderFactory
    {
        private readonly IList<IAllocationLogProvider> _allocationLogProviders;

        public DefaultAllocationLogProviderFactory(IList<IAllocationLogProvider> allocationLogProviders)
        {
            _allocationLogProviders = allocationLogProviders;
        }
        public async Task<IAllocationLogProvider> GetAllocationLogProviderAsync(string client)
        {
            foreach (var allocationLogProvider in _allocationLogProviders)
            {
                if (await allocationLogProvider.CanProvideAsync(client))
                    return allocationLogProvider;
            }

            return null;
        }
    }
}