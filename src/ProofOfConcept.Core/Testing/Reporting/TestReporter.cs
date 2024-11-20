using System.Diagnostics;

namespace ProofOfConcept.Core.Testing.Reporting;

public static class TestReporter
{
    private static Stopwatch _totalTimeStopwatch;
    private static Stopwatch _modelParsingTimeStopwatch;
    private static Stopwatch _parsingTemplatesTimeStopwatch;
    private static Stopwatch _testingTimeStopwatch;
    private static Stopwatch _timeToFindFirstFaultStopwatch;
    private static int _numberOfTestCasesGenerated;
    private static int _numberOfFaultsFound;
    private static TestVerdict _testVerdict;
    private static List<ITrace> _traces;
    private static string _failureReason;

    static TestReporter()
    {
        _totalTimeStopwatch = new Stopwatch();
        _modelParsingTimeStopwatch = new Stopwatch();
        _parsingTemplatesTimeStopwatch = new Stopwatch();
        _testingTimeStopwatch = new Stopwatch();
        _timeToFindFirstFaultStopwatch = new Stopwatch();
        _numberOfTestCasesGenerated = 0;
        _numberOfFaultsFound = 0;
        _failureReason = string.Empty;
        _traces = new List<ITrace>();
    }

    public static void ResetData()
    {
        _totalTimeStopwatch = new Stopwatch();
        _modelParsingTimeStopwatch = new Stopwatch();
        _parsingTemplatesTimeStopwatch = new Stopwatch();
        _testingTimeStopwatch = new Stopwatch();
        _timeToFindFirstFaultStopwatch = new Stopwatch();
        _numberOfTestCasesGenerated = 0;
        _numberOfFaultsFound = 0;
        _traces = new List<ITrace>();
    }
    
    public static void ProcessTimeEvent(TimeEvent input) 
    {
        switch (input)
        {
            case TimeEvent.ExecutionStarted:
                _totalTimeStopwatch.Start();
                return;
            case TimeEvent.ExecutionStopped:
                _totalTimeStopwatch.Stop();
                return;
            case TimeEvent.ModelParsingStarted:
                _modelParsingTimeStopwatch.Start();
                return;
            case TimeEvent.ModelParsingStopped:
                _modelParsingTimeStopwatch.Stop();
                return;
            case TimeEvent.ParsingTemplateStarted:
                _parsingTemplatesTimeStopwatch.Start();
                return;
            case TimeEvent.ParsingTemplateStopped:
                _parsingTemplatesTimeStopwatch.Stop();
                return;
            case TimeEvent.TestingStarted:
                _testingTimeStopwatch.Start();
                _timeToFindFirstFaultStopwatch.Start();
                return;
            case TimeEvent.TestingStopped:
                _testingTimeStopwatch.Stop();
                return;
            case TimeEvent.FirstFaultFound:
                _timeToFindFirstFaultStopwatch.Stop();
                return;
            case TimeEvent.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(input), input, null);
        }
    }
    
    public static void ProcessDataEvent(DataEvent input, int data) 
    {
        switch (input)
        {
            case DataEvent.TestCasesGenerated:
                _numberOfTestCasesGenerated += data;
                break;
            case DataEvent.FaultsDetected:
                _numberOfFaultsFound += data;
                break;
            case DataEvent.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(input), input, null);
        }
    }

    public static void ProcessInput(InputTrace inputTrace)
    {
        _traces.Add(inputTrace);
    }
    
    public static void ProcessOutput(OutputTrace outputTrace)
    {
        _traces.Add(outputTrace);
    }

    public static void ProcessTestResult(TestResult testVerdict)
    {
        _testVerdict = testVerdict.TestVerdict;
        _failureReason = testVerdict.Reason;
    }
    
    public static TestReport GenerateTestReport()
    {
        return new TestReport(
            _totalTimeStopwatch.ElapsedMilliseconds,
            _modelParsingTimeStopwatch.ElapsedMilliseconds,
            _parsingTemplatesTimeStopwatch.ElapsedMilliseconds,
            _testingTimeStopwatch.ElapsedMilliseconds,
            _timeToFindFirstFaultStopwatch.ElapsedMilliseconds,
            _numberOfTestCasesGenerated,
            _numberOfFaultsFound,
            _testVerdict,
            _failureReason,
            _traces,
            _traces.Count(x => x is InputTrace),
            _traces.Count(x => x is OutputTrace));
    }
}