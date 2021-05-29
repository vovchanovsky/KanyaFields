using System.Threading;
using System.Threading.Tasks;

namespace PasswordMediatorSvc.Application.Messaging
{
    public interface IAuthenticationSvcMessageHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}