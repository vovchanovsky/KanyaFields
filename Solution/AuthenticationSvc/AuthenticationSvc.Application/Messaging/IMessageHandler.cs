using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationSvc.Application.Messaging
{
    public interface IMessageHandler
    {
        Task HandleAsync(CancellationToken cancellationToken);
    }
}