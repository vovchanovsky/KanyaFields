using System.Threading;
using System.Threading.Tasks;
using AuthenticationSvc.Infrastructure.Infrastructure;
using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthenticationSvc.Infrastructure.Services
{
    public class CassandraInfrastructureInitializer : BackgroundService
    {
        public CassandraInfrastructureInitializer(
            ILogger<CassandraInfrastructureInitializer> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            _cluster = Cluster.Builder()
                .AddContactPoints(configuration[EnvironmentVariables.CassandraHost])
                .WithDefaultKeyspace("default")
                .WithCredentials(
                    configuration[EnvironmentVariables.CassandraUser],
                    configuration[EnvironmentVariables.CassandraPassword])
                .Build();

            _session = _cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CreateKeyspace();
            CreateTable();

            return Task.CompletedTask;
        }


        private void CreateKeyspace()
        {
            _logger.LogInformation($"Trying to create new KEYSPACE: {Keyspace}");

            var createKeyspaceQuery =
                $@"CREATE KEYSPACE {Keyspace} WITH replication = {{'class': 'SimpleStrategy', 'replication_factor':2}} AND durable_writes = true;";

            var createKeyspaceStatement = new SimpleStatement(createKeyspaceQuery);

            try
            {
                _session.Execute(createKeyspaceStatement);
                _logger.LogInformation($"Successfully created new KEYSPACE: {Keyspace}");
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogWarning(ex.Message);
                _logger.LogInformation($"KEYSPACE {Keyspace} is already exist");
            }
        }

        private void CreateTable()
        {
            _logger.LogInformation($"Trying to create new table: {Table}");

            var createTableQuery = $@"CREATE TABLE {Keyspace}.{Table}(
Username text,
Password text,
Claims text,
PRIMARY KEY (Username));";

            var createTableStatement = new SimpleStatement(createTableQuery);

            try
            {
                _session.Execute(createTableStatement);
                _logger.LogInformation($"Successfully created new table: {Table}");
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogWarning(ex.Message);
                _logger.LogInformation($"Table {Table} is already exist");
            }
        }


        private readonly ILogger<CassandraInfrastructureInitializer> _logger;
        private readonly Cluster _cluster;
        private readonly ISession _session;

        private const string Keyspace = "kanyafields_keyspace";
        private const string Table = "user_claims";
    }
}