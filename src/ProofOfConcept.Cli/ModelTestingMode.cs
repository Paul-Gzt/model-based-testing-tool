using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Core.Testing.Reporting;
using ProofOfConcept.Infrastructure;
using ProofOfConcept.Infrastructure.Generation;
using ProofOfConcept.Infrastructure.Microservices;
using ProofOfConcept.Infrastructure.Microservices.Input;
using ProofOfConcept.Infrastructure.Microservices.Output;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Redis;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Sql;
using ProofOfConcept.Infrastructure.Templates;
using ProofOfConcept.Infrastructure.Utility;
using HttpClient = ProofOfConcept.Infrastructure.Microservices.Protocol.Http.HttpClient;

namespace ProofOfConcept.Cli;

public static class ModelTestingMode
{
    public static async Task<int> ExecuteAsync(string modelPath)
    {
        var baseDirectory = Path.GetDirectoryName(modelPath) ?? "";

        TestReporter.ProcessTimeEvent(TimeEvent.ExecutionStarted);
        await TestAsync(modelPath, baseDirectory);
        TestReporter.ProcessTimeEvent(TimeEvent.ExecutionStopped);

        var testReport = TestReporter.GenerateTestReport();
        
        TestReportPrinter.PrintToConsole(testReport);
        await TestReportPrinter.PrintToFileAsync(testReport, baseDirectory);
        TestReporter.ResetData();
        
        return 0;
    }

    public static async Task TestAsync(string modelPath, string baseDirectory)
    {
        TestReporter.ProcessTimeEvent(TimeEvent.ModelParsingStarted);
        var specification = await ModelReader.ReadFromFileAsync(modelPath);
        TestReporter.ProcessTimeEvent(TimeEvent.ModelParsingStopped);
        
        var testPurpose = TestPurpose.GenerateFrom(specification);
        TestReporter.ProcessDataEvent(DataEvent.TestCasesGenerated, 1);

        var testVerdict = await TestSutAsync(specification, testPurpose, baseDirectory);

        if (testVerdict.TestVerdict == TestVerdict.Failed)
        {
            TestReporter.ProcessDataEvent(DataEvent.FaultsDetected, 1);
        }
        
        TestReporter.ProcessTestResult(testVerdict);
    }

    // TODO: Dependency injection
    private static async Task<TestResult> TestSutAsync(Specification specification, TestPurpose testPurpose,
        string baseDirectory)
    {
        var testingContext = new TestingContext
        {
            BaseDirectory = baseDirectory
        };

        var httpOutputAdapter = new HttpOutputAdapter();
        var sqlOutputAdapter = new SqlOutputAdapter();
        var outputAdapters = new List<IOutputAdapter>
        {
            httpOutputAdapter,
            sqlOutputAdapter
        };

        var inputAdapters = new List<IInputAdapter>
        {
            new HttpInputAdapter(new HttpClient(), testingContext).SetOutputAdapter(httpOutputAdapter),
            new RedisInputAdapter(new RedisClient()),
            new SqlInputAdapter(new SqlClient("Server=tcp:127.0.0.1,5433;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Geheim_123;TrustServerCertificate=True;"), testingContext).SetOutputAdapter(sqlOutputAdapter)
        };

        if (specification.InitialVariables.Any(x => x.Name.Contains("queueName", StringComparison.InvariantCultureIgnoreCase)))
        {
            var messagePublisher = new MessagePublisher();
            inputAdapters.Add(new AmqpInputAdapter(messagePublisher, testingContext));
            var amqpConfigurations = specification.GetAmqpConfiguration();
            
            outputAdapters.AddRange(amqpConfigurations.Select(config=> new AmqpOutputAdapter(config.QueueName, config.ExchangeName , config.TopicName)));
        }

        var inputChain = new AdapterChain<IInputAdapter>(inputAdapters);
        var outputChain = new AdapterChain<IOutputAdapter>(outputAdapters);

        var sut = new Microservice(inputChain, outputChain);
        var asserter = new JsonTemplateAsserter(baseDirectory);
        var inputGenerator = new InputGenerator();
        var algorithm = new Algorithm1(inputGenerator);

        TestReporter.ProcessTimeEvent(TimeEvent.TestingStarted);

        var specificationUnderTest = new SpecificationUnderTest(specification, specification.CurrentState, asserter);
        var result = await algorithm.ExecuteAsync(specificationUnderTest, testPurpose, sut);
        TestReporter.ProcessTimeEvent(TimeEvent.TestingStopped);
        TestReporter.ProcessTimeEvent(TimeEvent.FirstFaultFound);
        
        return result;
    }
}