using System.Threading.Tasks;

namespace ApiSvc.InfrastructureInterfaces.Clients.IdentityProviderClient
{
    public interface IIdentityProviderClient
    {
        Task RegisterUser(string username, string password);

        Task<string> Authenticate(string username, string password);
    }
}