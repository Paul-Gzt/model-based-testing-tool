namespace ProofOfConcept.Core.Testing.Reporting;

public readonly record struct TestReport(
    long TotalTimeInMilliseconds, 
    long ModelParsingTimeInMilliseconds, 
    long ParsingTemplatesTimeInMilliseconds, 
    long TestingTimeInMilliseconds, 
    long TimeToFindFirstFaultInMilliseconds, 
    int NumberOfTestCasesGenerated, 
    int NumberOfFaultsFound,
    TestVerdict TestVerdict,
    string FailureReason,
    List<ITrace> Traces,
    int NumberOfInputsGenerated,
    int NumberOfOutputsObserved);