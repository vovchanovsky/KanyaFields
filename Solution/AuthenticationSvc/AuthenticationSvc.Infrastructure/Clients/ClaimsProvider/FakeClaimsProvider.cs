using System.Threading.Tasks;
using AuthenticationSvc.Domain.Models;
using AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider;

namespace AuthenticationSvc.Infrastructure.Clients.ClaimsProvider
{
    public class FakeClaimsProvider : IUserClaimsProvider
    {
        public Task<User> GetUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UserIsAlreadyExist(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}