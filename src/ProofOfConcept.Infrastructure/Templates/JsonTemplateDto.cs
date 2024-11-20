using Newtonsoft.Json.Linq;

namespace ProofOfConcept.Infrastructure.Templates;

public class JsonTemplateDto
{
    public AssertionMode Assertion { get; set; } = AssertionMode.Unknown;

    public JToken Body { get; set; } = null!;
}