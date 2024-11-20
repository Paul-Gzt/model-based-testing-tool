using System.Diagnostics;
using ProofOfConcept.Cli;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Core.Testing.Reporting;
using ProofOfConcept.Infrastructure;

namespace Ordering.Experiments;

[TestClass]
public class UnitTest1
{
    // 0, 20
    private readonly List<int> Mutants = Enumerable.Range(0, 20).ToList();

    private void ActivateMutant(int mutantId)
    {
        var path =
            "C:\\Development\\uva\\Experiments\\eShopOnContainers\\src\\Services\\Basket\\Basket.API\\bin\\Debug\\net6.0\\mutants\\active_mutants.txt";

        File.WriteAllText(path, $"{mutantId}");
    }
    
    [TestMethod]
    public async Task OrderStarted_ConformingSUT_Passes()
    {
        const int mutant = 0;
        Console.WriteLine($"Testing mutant: {mutant}");
        ActivateMutant(mutant);

        // Arrange
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var modelPath = Path.Combine(baseDirectory, "Models", "OrderStarted.model");

        // Act
        await ModelTestingMode.TestAsync(modelPath, "");

        var testReport = TestReporter.GenerateTestReport();
        TestReportPrinter.PrintToConsole(testReport);
        Console.WriteLine($"TestVerdict for mutant {mutant} - {testReport.TestVerdict}");
        Assert.AreEqual(TestVerdict.Passed, testReport.TestVerdict);
    }
    
    [TestMethod]
    public async Task GracePeriod_ConformingSUT_Passes()
    {
        const int mutant = 0;
        Console.WriteLine($"Testing mutant: {mutant}");
        ActivateMutant(mutant);

        // Arrange
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var modelPath = Path.Combine(baseDirectory, "Models", "GracePeriod.model");

        // Act
        await ModelTestingMode.TestAsync(modelPath, "");

        var testReport = TestReporter.GenerateTestReport();
        TestReportPrinter.PrintToConsole(testReport);
        Console.WriteLine($"TestVerdict for mutant {mutant} - {testReport.TestVerdict}");
        Assert.AreEqual(TestVerdict.Passed, testReport.TestVerdict);
    }
}