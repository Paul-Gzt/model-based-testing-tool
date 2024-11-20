using System.Diagnostics;
using ProofOfConcept.Cli;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Core.Testing.Reporting;
using ProofOfConcept.Infrastructure;

namespace Catalog.Experiments;

[TestClass]
public class UnitTest1
{
    private Process _process = null!;

    // 0, 20
    private readonly List<int> Mutants = Enumerable.Range(0, 20).ToList();

    [TestInitialize]
    public void TestInitialize()
    {
        foreach (var node in Process.GetProcessesByName("Basket.API"))
        {
            node.Kill();
        }

        _process = new Process();
        _process.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
        _process.StartInfo.WorkingDirectory =
            "C:\\Development\\uva\\Experiments\\eShopOnContainers\\src\\Services\\Basket\\Basket.API";
        _process.StartInfo.Arguments = @"/C dotnet run";
        _process.Start();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        foreach (var node in Process.GetProcessesByName("Basket.API"))
        {
            node.Kill();
        }

        _process.Kill(true);
    }

    private void ActivateMutant(int mutantId)
    {
        var path =
            "C:\\Development\\uva\\Experiments\\eShopOnContainers\\src\\Services\\Basket\\Basket.API\\bin\\Debug\\net6.0\\mutants\\active_mutants.txt";

        File.WriteAllText(path, $"{mutantId}");
    }
    
    [TestMethod]
    public async Task Test_ConformingSUT_Passes()
    {
        const int mutant = 0;
        Console.WriteLine($"Testing mutant: {mutant}");
        ActivateMutant(mutant);

        // Arrange
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var modelPath = Path.Combine(baseDirectory, "Models", "Order.model");

        // Act
        await ModelTestingMode.TestAsync(modelPath, "");

        var testReport = TestReporter.GenerateTestReport();
        TestReportPrinter.PrintToConsole(testReport);
        Console.WriteLine($"TestVerdict for mutant {mutant} - {testReport.TestVerdict}");
        Assert.AreEqual(TestVerdict.Passed, testReport.TestVerdict);
    }
}