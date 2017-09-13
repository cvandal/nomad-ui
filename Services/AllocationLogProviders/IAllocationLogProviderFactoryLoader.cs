using System.Threading.Tasks;

namespace Nomad.Services.AllocationLogProviders
{
    public interface IAllocationLogProviderFactoryLoader
    {
        Task<IAllocationLogProviderFactory> GetAllocationLogProviderFactoryAsync();
    }
}