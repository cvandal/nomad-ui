using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public class DefaultAllocationLogProviderFactoryLoader : IAllocationLogProviderFactoryLoader
    {
        private readonly IList<AllocationLogsProviderClientBinding> _AllocationLogsProviderClientBindings;
        
        public DefaultAllocationLogProviderFactoryLoader(IList<AllocationLogsProviderClientBinding> AllocationLogsProviderClientBindings)
        {
            _AllocationLogsProviderClientBindings = AllocationLogsProviderClientBindings ?? 
                new List<AllocationLogsProviderClientBinding>();
        }

        public async Task<IAllocationLogProviderFactory> GetAllocationLogProviderFactoryAsync()
        {
            IList<IAllocationLogProvider> providers = new List<IAllocationLogProvider>();

            foreach (var AllocationLogsProviderClientBinding in _AllocationLogsProviderClientBindings)
            {
                var providerAssembly = Assembly.Load(new AssemblyName(AllocationLogsProviderClientBinding.AllocationLogsProviderAssemblyInfo.AssemblyName));
                var providerInstance = providerAssembly?.CreateInstance(AllocationLogsProviderClientBinding.AllocationLogsProviderAssemblyInfo
                    .FullyQualifiedClassName) as IAllocationLogProvider;

                await providerInstance.AssignClientsAsync(AllocationLogsProviderClientBinding.Clients)
                    .ConfigureAwait(false);
                
                providers.Add(providerInstance);

            }

            return await Task.FromResult<IAllocationLogProviderFactory>(new DefaultAllocationLogProviderFactory(providers));
        }
    }
}
