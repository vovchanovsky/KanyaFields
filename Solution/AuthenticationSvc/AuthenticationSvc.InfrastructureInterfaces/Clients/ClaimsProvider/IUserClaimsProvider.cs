using System.Threading.Tasks;
using AuthenticationSvc.Domain.Models;

namespace AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider
{
    public interface IUserClaimsProvider
    {
        Task<User> GetUser(string username);

        Task CreateUser(User user);

        Task<bool> UserIsAlreadyExist(string username);
    }
}