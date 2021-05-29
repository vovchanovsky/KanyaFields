using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PasswordProviderSvc.Domain.Entities;
using PasswordProviderSvc.Infrastructure.Infrastructure;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;

namespace PasswordProviderSvc.Infrastructure.Clients.StorageProvider
{
    public class CassandraPasswordStorageProvider : IPasswordStorageProvider
    {
        public CassandraPasswordStorageProvider(
            ILogger<CassandraPasswordStorageProvider> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            _cluster = Cluster.Builder()
                .AddContactPoints(configuration[EnvironmentVariables.CassandraHost])
                .WithDefaultKeyspace(Keyspace)
                .WithCredentials(
                    configuration[EnvironmentVariables.CassandraUser],
                    configuration[EnvironmentVariables.CassandraPassword])
                .Build();

            _session = _cluster.Connect(Keyspace);
        }

        public Task<IEnumerable<PasswordItem>> GetPasswords(Guid userId)
        {
            var passwordsTable = new Table<PasswordItem>(_session, MappingConfiguration.Global, Table);

            return passwordsTable
                .Where(p => p.UserId == userId)
                .ExecuteAsync();
        }

        public Task<PasswordItem> GetPasswordItem(Guid userId, Guid id)
        {
            var passwordsTable = new Table<PasswordItem>(_session, MappingConfiguration.Global, Table);

            return passwordsTable
                .FirstOrDefault(p => p.UserId == userId && p.PasswordId == id)
                .ExecuteAsync();
        }

        public Task DeletePassword(Guid userId, Guid id)
        {
            var passwordsTable = new Table<PasswordItem>(_session, MappingConfiguration.Global, Table);

            return passwordsTable
                .Where(p => p.UserId == userId && p.PasswordId == id)
                .Delete()
                .SetConsistencyLevel(ConsistencyLevel.Two)
                .ExecuteAsync();
        }

        public async Task<Guid> InsertPassword(Guid userId, PasswordItem passwordItem)
        {
            var passwordsTable = new Table<PasswordItem>(_session, MappingConfiguration.Global, Table);

            await passwordsTable
                .Insert(passwordItem)
                .SetConsistencyLevel(ConsistencyLevel.Two)
                .ExecuteAsync();

            // TODO: refactor this weird shit
            return passwordItem.PasswordId;
        }


        private readonly ILogger<CassandraPasswordStorageProvider> _logger;
        private readonly Cluster _cluster;
        private readonly ISession _session;

        private const string Keyspace = "kanyafields_keyspace";
        private const string Table = "password_items";
    }
}