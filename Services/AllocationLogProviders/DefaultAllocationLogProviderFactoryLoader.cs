using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public class DefaultAllocationLogProviderFactoryLoader : IAllocationLogProviderFactoryLoader
    {
        private readonly IList<AllocaltionLogsProviderClientBinding> _allocaltionLogsProviderClientBindings;
        
        public DefaultAllocationLogProviderFactoryLoader(
            IList<AllocaltionLogsProviderClientBinding> allocaltionLogsProviderClientBindings)
        {
            _allocaltionLogsProviderClientBindings = allocaltionLogsProviderClientBindings ?? 
                new List<AllocaltionLogsProviderClientBinding>();
        }
        public async Task<IAllocationLogProviderFactory> GetAllocationLogProviderFactoryAsync()
        {
            IList<IAllocationLogProvider> providers = new List<IAllocationLogProvider>();
            foreach (var allocaltionLogsProviderClientBinding in _allocaltionLogsProviderClientBindings)
            {
                var providerAssembly = 
                    Assembly.Load(new AssemblyName(allocaltionLogsProviderClientBinding.AllocaltionLogsProviderAssemblyInfo.AssemblyName));
                var providerInstance = providerAssembly?.CreateInstance(allocaltionLogsProviderClientBinding.AllocaltionLogsProviderAssemblyInfo
                    .FullyQualifiedClassName) as IAllocationLogProvider;

                // ReSharper disable once PossibleNullReferenceException
                await providerInstance.AssignClientsAsync(allocaltionLogsProviderClientBinding.Clients).
                    ConfigureAwait(false);
                
                providers.Add(providerInstance);

            }

            return await Task.FromResult<IAllocationLogProviderFactory>(new DefaultAllocationLogProviderFactory(providers));
        }
    }
}