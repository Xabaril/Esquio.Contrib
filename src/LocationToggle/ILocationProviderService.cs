using System.Threading;
using System.Threading.Tasks;

namespace LocationToggles
{
    public interface ILocationProviderService
    {
        Task<string> GetCountryName(string ipaddress, CancellationToken cancellationToken = default);

        Task<string> GetCountryCode(string ipaddress, CancellationToken cancellationToken = default);
    }
}
