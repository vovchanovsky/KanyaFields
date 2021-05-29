using System;
using System.Threading.Tasks;

namespace ApiSvc.InfrastructureInterfaces.Services
{
    public interface ISynchronizer
    {
        public Guid ApplicationInstanceId { get; }

        Guid RegisterSynchronizer();

        Task WaitOneAsync(Guid id);

        Task CreateAndWaitAsync(Guid id);

        void ReleaseOne(Guid id);
    }
}