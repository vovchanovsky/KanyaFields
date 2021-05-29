using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.Infrastructure.Resources;
using ApiSvc.InfrastructureInterfaces.Services;
using Microsoft.Extensions.Logging;

namespace ApiSvc.Infrastructure.Services
{
    public class Synchronizer : ISynchronizer
    {
        public Synchronizer(ILogger<Synchronizer> logger)
        {
            _synchronizerDictionary = new ConcurrentDictionary<Guid, ManualResetEventSlim>();
            _logger = logger;
            ApplicationInstanceId = Guid.NewGuid();
        }

        public Guid ApplicationInstanceId { get; }


        public Guid RegisterSynchronizer()
        {
            var (id, synchronizer) = CreateSynchronizer();
            RegisterSynchronizer(id, synchronizer);

            return id;
        }

        public async Task WaitOneAsync(Guid id)
        {
            if (_synchronizerDictionary.TryGetValue(id, out ManualResetEventSlim synchronizer) is false)
            {
                var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessages.SynchronizerNotFound, id);

                _logger.LogError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            await Task.Run(() => WaitForSynchronizer(synchronizer)).ConfigureAwait(false);
        }

        /// <param name="id">should be created outside</param>
        public async Task CreateAndWaitAsync(Guid id)
        {
            var synchronizer = new ManualResetEventSlim();
            RegisterSynchronizer(id, synchronizer);
            await Task.Run(() => WaitForSynchronizer(synchronizer)).ConfigureAwait(false);
        }

        public void ReleaseOne(Guid id)
        {
            if (_synchronizerDictionary.TryGetValue(id, out ManualResetEventSlim synchronizer) is false)
            {
                var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessages.SynchronizerNotFound, id);

                _logger.LogError(errorMessage);
                //throw new ApplicationException(errorMessage);
                return;
            }

            synchronizer.Set();
            _synchronizerDictionary.TryRemove(id, out _);
        }


        private static Task WaitForSynchronizer(ManualResetEventSlim synchronizer)
        {
            synchronizer.Wait(Timeout);
            return Task.CompletedTask;
        }

        private void RegisterSynchronizer(Guid id, ManualResetEventSlim synchronizer)
        {
            if (_synchronizerDictionary.TryAdd(id, synchronizer) is false)
            {
                var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessages.SynchronizerCreationError, id);

                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }
        }

        private static (Guid id, ManualResetEventSlim synchronizer) CreateSynchronizer() =>
            (Guid.NewGuid(), new ManualResetEventSlim());

        private readonly ConcurrentDictionary<Guid, ManualResetEventSlim> _synchronizerDictionary;
        private readonly ILogger<Synchronizer> _logger;
        private const int Timeout = 10000;
    }
}