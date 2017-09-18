using System.Collections.Generic;

namespace Nomad.Services.AllocationLogProviders
{
    public class AllocationLogsProviderClientBinding
    {
        public AllocationLogsProviderAssemblyInfo AllocationLogsProviderAssemblyInfo { get; set; }
        public IList<string> Clients { get; set; }
    }
}
