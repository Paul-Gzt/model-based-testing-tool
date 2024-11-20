namespace ProofOfConcept.Core.Testing.Reporting;

public enum TimeEvent
{
    Unknown = 0,
    ExecutionStarted = 1,
    ExecutionStopped = 2,
    ModelParsingStarted = 3,
    ModelParsingStopped = 4,
    ParsingTemplateStarted = 5,
    ParsingTemplateStopped = 6,
    TestingStarted = 7,
    TestingStopped = 8,
    FirstFaultFound = 9
}
