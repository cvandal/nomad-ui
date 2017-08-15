using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public class AllocationLogProviderFactoryLoader : IAllocationLogProviderFactoryLoader
    {
        public IList<AllocaltionLogsProviderClientBinding> AllocaltionLogsProviderClientBindings { get; set; } = 
            new List<AllocaltionLogsProviderClientBinding>();

        public async Task<IAllocationLogProviderFactory> GetAllocationLogProviderFactoryAsync()
        {
            IList<IAllocationLogProvider> providers = new List<IAllocationLogProvider>();
            foreach (var allocaltionLogsProviderClientBinding in AllocaltionLogsProviderClientBindings)
            {
                var providerAssembly = 
                    Assembly.Load(new AssemblyName(allocaltionLogsProviderClientBinding.AllocaltionLogsProviderAssemblyInfo.AssemblyName));
                var providerInstance = providerAssembly?.CreateInstance(allocaltionLogsProviderClientBinding.AllocaltionLogsProviderAssemblyInfo
                    .FullyQualifiedClassName) as IAllocationLogProvider;

                // ReSharper disable once PossibleNullReferenceException
                providerInstance.AssignClientsAsync(allocaltionLogsProviderClientBinding.Clients);
                
                providers.Add(providerInstance);

            }

            return await Task.FromResult<IAllocationLogProviderFactory>(new DefaultAllocationLogProviderFactory(providers));
        }
    }
}