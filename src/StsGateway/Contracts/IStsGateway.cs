using System.Threading.Tasks;

namespace StsGateway.Contracts
{
    public interface IStsGateway
    {
        Task<string?> GetAccessTokenAsync();
    }
}
