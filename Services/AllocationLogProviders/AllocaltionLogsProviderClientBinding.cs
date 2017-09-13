using System.Collections.Generic;

namespace Nomad.Services.AllocationLogProviders
{
    public class AllocaltionLogsProviderClientBinding
    {
        public AllocaltionLogsProviderAssemblyInfo AllocaltionLogsProviderAssemblyInfo { get; set; }
        public IList<string> Clients { get; set; }
    }
}