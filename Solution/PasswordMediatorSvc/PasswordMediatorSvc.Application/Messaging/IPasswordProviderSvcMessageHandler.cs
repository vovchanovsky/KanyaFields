using System.Threading;
using System.Threading.Tasks;

namespace PasswordMediatorSvc.Application.Messaging
{
    public interface IPasswordProviderSvcMessageHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}