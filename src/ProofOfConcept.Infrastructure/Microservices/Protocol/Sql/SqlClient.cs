using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Utility;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Sql;

public class SqlClient : ISqlClient
{
    private readonly string _connectionString;
    
    public SqlClient(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<Trace> FlushAllData()
    {
        var tableNames = new List<string>
        {
            "dbo.IntegrationEventLog",
            "ordering.orderItems",
            "ordering.orders",
            "ordering.requests",
            "ordering.buyers",
        };

        await using var connection = new SqlConnection(_connectionString);
        foreach (var tableName in tableNames)
        {
            await connection.ExecuteAsync($"DELETE FROM {tableName};");
        }

        return new Trace(new Gate(ActionType.Unknown, string.Empty), string.Empty);
    }

    public async Task<Trace> Query(string query)
    {
        await using var connection = new SqlConnection(_connectionString);

        var databaseResult = await connection.QueryAsync(query);
        
        var result = DapperUtility.DapperQueryToCsvString(databaseResult);
        
        return new Trace(new Gate(ActionType.Output, "!sql_result"), JsonConvert.SerializeObject(new CsvResult(result)));
    }

    public async Task Seed(string seedScript)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(seedScript);
    }
}