using System.Text;
using ProofOfConcept.Core.Testing.Reporting;

namespace ProofOfConcept.Infrastructure;

public static class TestReportPrinter
{
    public static async Task PrintToFileAsync(TestReport testReport, string directory)
    {
        var text = Print(testReport);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var today = DateTime.Now;
        var path = Path.Combine(directory, $"test-report-{today:yyyy-MM-dd-HH-mm-ss}.txt");
        await using var fileStream = File.Create(path);
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(fileStream);
    }
    
    public static void PrintToConsole(TestReport testReport)
    {
        var text = Print(testReport);
        
        Console.WriteLine(text);
    }
    
    private static string Print(TestReport testReport)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test Run on: '{DateTime.Now:u}'");
        stringBuilder.AppendLine($"Verdict: '{testReport.TestVerdict}'");
        stringBuilder.AppendLine($"Reason: '{testReport.FailureReason}'");
        stringBuilder.AppendLine($"Total time in milliseconds: '{testReport.TotalTimeInMilliseconds}'");
        stringBuilder.AppendLine($"Model parsing time in milliseconds: '{testReport.ModelParsingTimeInMilliseconds}'");
        stringBuilder.AppendLine($"Parsing templates time in milliseconds: '{testReport.ParsingTemplatesTimeInMilliseconds}'");
        stringBuilder.AppendLine($"Testing time in milliseconds: '{testReport.TestingTimeInMilliseconds}'");
        stringBuilder.AppendLine($"Time to find first fault in milliseconds: '{testReport.TimeToFindFirstFaultInMilliseconds}'");
        stringBuilder.AppendLine($"Number of test cases generated: '{testReport.NumberOfTestCasesGenerated}'");
        stringBuilder.AppendLine($"Number of faults found: '{testReport.NumberOfFaultsFound}'");
        stringBuilder.AppendLine($"Number of inputs generated: '{testReport.NumberOfInputsGenerated}'");
        stringBuilder.AppendLine($"Number of outputs observed: '{testReport.NumberOfOutputsObserved}'");

        stringBuilder.AppendLine("****** Traces ******");
        foreach (var trace in testReport.Traces)
        {
            stringBuilder.AppendLine(trace.ToString());
        }

        return stringBuilder.ToString();
    }
}