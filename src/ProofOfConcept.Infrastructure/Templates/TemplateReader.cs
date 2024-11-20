using ProofOfConcept.Core.Testing.Reporting;

namespace ProofOfConcept.Infrastructure.Templates;

public static class TemplateReader
{
    private const string TemplateDirectory = "templates";

    public static string GetTemplateName(string variable)
    {
        return variable[Constants.TemplatePrefix.Length..];
    }
    
    public static async Task<string> GetTemplateAsync(string baseDirectory, string templateName)
    {
        var path = Path.Combine(baseDirectory, TemplateDirectory, $"{templateName}.json");
        
        TestReporter.ProcessTimeEvent(TimeEvent.ParsingTemplateStarted);
        var template = await File.ReadAllTextAsync(path);
        TestReporter.ProcessTimeEvent(TimeEvent.ParsingTemplateStopped);

        return template;
    }
    
    public static async Task<string> GetSqlScriptAsync(string baseDirectory, string templateName)
    {
        var path = Path.Combine(baseDirectory, TemplateDirectory, $"{templateName}.sql");
        
        TestReporter.ProcessTimeEvent(TimeEvent.ParsingTemplateStarted);
        var template = await File.ReadAllTextAsync(path);
        TestReporter.ProcessTimeEvent(TimeEvent.ParsingTemplateStopped);

        return template;
    }
}