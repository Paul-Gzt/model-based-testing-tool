using FluentAssertions;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Core.Testing.Reporting;

namespace ProofOfConcept.Cli.Test.Microservices;

[TestClass]
public class MicroserviceTests
{
    [DataRow("Test_Get_ReturnsOk.model")]
    [DataRow("Test_Post_ReturnsOk.model")]
    [DataRow("Test_Unknown_ReturnsNotFound.model")]
    [DataRow("Test_Amqp_Pubsub_Single.model")]
    [DataTestMethod]
    public async Task TestVerdict_Passed(string modelFileName)
    {
        // Arrange
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var modelPath = Path.Combine(baseDirectory, "Models", modelFileName);
        
        // Act
        await ModelTestingMode.TestAsync(modelPath, "Models");

        var testReport = TestReporter.GenerateTestReport();
        
        // Assert
        testReport.TestVerdict.Should().Be(TestVerdict.Passed);
    }
}