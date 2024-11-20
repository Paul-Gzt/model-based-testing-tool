using System.Dynamic;
using System.Globalization;
using System.Text;
using CsvHelper;
using JsonDiffer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProofOfConcept.Core.Assertion;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Utility;

namespace ProofOfConcept.Infrastructure.Templates;

public class JsonTemplateAsserter : IAsserter
{
    private readonly string _baseDirectory;
    public JsonTemplateAsserter(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }
    
    // TODO: Does not work properly with async traces
    /// <summary>
    /// Given the observed data and the current state of the specification, determine which switch transition can be
    /// made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="@switch"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AssertionResult> AssertAsync(string value, Switch @switch)
    {
        var guards = @switch.Guards;
        if (!guards.Any()) return new AssertionResult(true, string.Empty);
        
        var guardsWithTemplate = guards.Where(x => x.RightOperand.StartsWith(Constants.TemplatePrefix)).ToList();

        var results = new List<AssertionResult>();
        foreach (var guardWithTemplate in guardsWithTemplate)
        {
            var result = await AssertGuard(value, guardWithTemplate);
            results.Add(result);
        }

        if (results.Any(r => r.Success)) return new AssertionResult(true, string.Empty);

        var stringBuilder = new StringBuilder();

        results.ForEach(r => stringBuilder.AppendLine(r.Reason));

        return new AssertionResult(false, stringBuilder.ToString());
    }

    // We need some kind of flow where the asserter has to determine which location should be instantiated
    // TODO: Consider splitting each assertionmode in a separate asserter, this class is getting large
    private async Task<AssertionResult> AssertGuard(string value, Guard guard)
    { 
        if (guard.Operation == Operation.Unknown) return new AssertionResult(true, string.Empty);
        
        var templateName = TemplateReader.GetTemplateName(guard.RightOperand);
        var template = await TemplateReader.GetTemplateAsync(_baseDirectory, templateName);
        var templateDto = JsonConvert.DeserializeObject<JsonTemplateDto>(template)!;

        if (templateDto.Assertion == AssertionMode.Exact)
        {
            return AssertJsonExactEqual(value, templateDto);
        }

        if (templateDto.Assertion == AssertionMode.Csv)
        {
            return AssertCsv(value, templateDto);
        }

        if (templateDto.Assertion == AssertionMode.Custom)
        {
            return AssertJsonCustomEqual(value, templateDto);
        }

        return new AssertionResult(false, "Unsupported AssertionMode");
    }

    private AssertionResult AssertJsonCustomEqual(string value, JsonTemplateDto templateDto)
    {
        var (newTemplateJtoken, newExpectedValueJtoken) = ApplyAnyAssertions(templateDto.Body, JToken.Parse(value));
        if (string.IsNullOrEmpty(value)) return new AssertionResult(false, "We can't assert empty bodies");
        ;

        var diff = JsonDifferentiator.Differentiate(newTemplateJtoken, newExpectedValueJtoken, OutputMode.Detailed);

        if (diff is null) return new AssertionResult(true, string.Empty);

        return new AssertionResult(false, diff.ToString());
    }

    private static AssertionResult AssertCsv(string value, JsonTemplateDto templateDto)
    {
        if (string.IsNullOrEmpty(value)) return new AssertionResult(false, "We can't assert empty bodies");

        var incomingCsvResult = JsonConvert.DeserializeObject<CsvResult>(value)?.Value;
        var templateCsvResult = JsonConvert.DeserializeObject<CsvResult>(templateDto.Body.ToString())?.Value;

        if (incomingCsvResult is null || templateCsvResult is null) return new AssertionResult(false, "Parsing failed");

        var equal = CompareCsv(incomingCsvResult, templateCsvResult);

        return new AssertionResult(equal, "");
    }

    private static AssertionResult AssertJsonExactEqual(string value, JsonTemplateDto templateDto)
    {
        if (string.IsNullOrEmpty(value)) return new AssertionResult(false, "We can't assert empty bodies");

        var diff = JsonDifferentiator.Differentiate(templateDto.Body, JToken.Parse(value), OutputMode.Detailed);

        if (diff is null) return new AssertionResult(true, string.Empty);

        return new AssertionResult(false, diff.ToString());
    }

    private static bool CompareCsv(string incomingCsvResult, string templateCsvResult)
    {
        List<dynamic> incomingRecords;
        List<dynamic> templateRecords;

        using (var reader = new StringReader(incomingCsvResult))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            incomingRecords = csv.GetRecords<dynamic>().ToList();
        }
        
        using (var reader = new StringReader(templateCsvResult))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            templateRecords = csv.GetRecords<dynamic>().ToList();
        }

        for (var i = 0; i < incomingRecords.Count; i++)
        {
            ExpandoObject incomingRecord = incomingRecords[i];
            ExpandoObject templateRecord = templateRecords[i];

            for (var j = 0; j < incomingRecord.Count(); j++)
            {
                var incomingCell = incomingRecord.ElementAt(j);
                var templateCell = templateRecord.ElementAt(j);

                if ((string) templateCell.Value! == "a::any") continue;
                
                var equals =  incomingCell.Equals(templateCell);
                
                if (!equals) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Simply ignores fields in JToken
    /// </summary>
    private (JToken newTemplateJtoken, JToken newExpectedValueJtoken) ApplyAnyAssertions(JToken templateDtoJtoken, JToken expectedValueJtoken)
    {
        var (newTemplateJtoken, removedFields) = templateDtoJtoken.RemoveValues(new []{ "a::any" });
        var newExpectedValueJtoken = expectedValueJtoken.RemoveFields(removedFields);

        return (newTemplateJtoken, newExpectedValueJtoken);
    }
}