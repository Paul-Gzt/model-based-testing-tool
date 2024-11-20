using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Sql;
using ProofOfConcept.Infrastructure.Templates;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public class SqlInputAdapter : InputAdapterBase
{
    private static readonly List<string> SupportedLabels = new()
    {
        "?sql_flush_all_data",
        "?sql_query",
        "?sql_seed"
    };

    private readonly ISqlClient _sqlClient;
    private readonly TestingContext _testingContext;
    
    public SqlInputAdapter(ISqlClient sqlClient, TestingContext testingContext) : base(SupportedLabels)
    {
        _sqlClient = sqlClient;
        _testingContext = testingContext;
    }

    protected override async Task<Trace?> DoAction(string label, List<Parameter> parameters)
    {
        return label switch
        {
            "?sql_flush_all_data" => await _sqlClient.FlushAllData(),
            "?sql_query" => await PerformSqlQuery(parameters),
            "?sql_seed" => await PerformSqlSeed(parameters),
            _ => throw new NotImplementedException($"Unsupported label: {label}")
        };
    }

    private async Task<Trace?> PerformSqlQuery(List<Parameter> parameters)
    {
        var query = parameters.FirstOrDefault(p => p.Name == "query").Value;

        return await _sqlClient.Query(query);
    }
    
    private async Task<Trace?> PerformSqlSeed(List<Parameter> parameters)
    {
        var sqlScriptName = parameters.FirstOrDefault(p => p.Name == "script").Value;
        var sqlScriptFileName = TemplateReader.GetTemplateName(sqlScriptName);
        var sqlContents = await TemplateReader.GetSqlScriptAsync(_testingContext.BaseDirectory, sqlScriptFileName);

        await _sqlClient.Seed(sqlContents);
        
        return null;
    }
}