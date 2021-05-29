using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationSvc.Domain.Models;
using AuthenticationSvc.Infrastructure.Infrastructure;
using AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthenticationSvc.Infrastructure.Clients.ClaimsProvider
{
    public class CassandraUserClaimsProvider : IUserClaimsProvider
    {
        public CassandraUserClaimsProvider(
            ILogger<CassandraUserClaimsProvider> logger,
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

        public Task<User> GetUser(string username)
        {
            var userTable = new Table<User>(_session, MappingConfiguration.Global, Table);

            return userTable
                .First(u => u.Username == username)
                .ExecuteAsync();
        }

        public Task CreateUser(User user)
        {
            var userTable = new Table<User>(_session, MappingConfiguration.Global, Table);

            return userTable
                .Insert(user)
                .SetConsistencyLevel(ConsistencyLevel.Two)
                .ExecuteAsync();
        }

        public async Task<bool> UserIsAlreadyExist(string username)
        {
            var userTable = new Table<User>(_session, MappingConfiguration.Global, Table);
            var user = await userTable
                .FirstOrDefault(u => u.Username == username)
                .ExecuteAsync();

            return user is not null;
        }


        private readonly ILogger<CassandraUserClaimsProvider> _logger;
        private readonly Cluster _cluster;
        private readonly ISession _session;

        private const string Keyspace = "kanyafields_keyspace";
        private const string Table = "user_claims";
    }
}