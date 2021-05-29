using System.Threading;
using System.Threading.Tasks;

namespace PasswordProviderSvc.Application.Messaging
{
    public interface IMessageHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}