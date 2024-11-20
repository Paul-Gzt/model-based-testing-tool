using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Http;
using ProofOfConcept.Infrastructure.Templates;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public class HttpInputAdapter : InputAdapterBase
{
    private static readonly List<string> SupportedLabels = new()
    {
        "?http_get",
        "?http_post",
        "?http_put",
        "?http_delete"
    };
    
    private readonly IHttpClient _httpClient;
    private readonly TestingContext _testingContext;

    public HttpInputAdapter(IHttpClient httpClient, TestingContext testingContext): base(SupportedLabels)
    {
        _httpClient = httpClient;
        _testingContext = testingContext;
    }

    protected override async Task<Trace?> DoAction(string label, List<Parameter> parameters)
    {
        return label switch
        {
            "?http_get" => await PerformGetAsync(parameters),
            "?http_post" => await PerformPostAsync(parameters),
            "?http_put" => await PerformPutAsync(parameters),
            "?http_delete" => await PerformDeleteAsync(parameters),
            _ => throw new NotImplementedException($"Unsupported label: {label}")
        };
    }
    
    private async Task<Trace> PerformGetAsync(List<Parameter> parameters)
    {
        var endpoint = parameters.FirstOrDefault(p => p.Name == "endpoint").Value;
        
        return await _httpClient.GetAsync(endpoint);
    }

    private async Task<Trace> PerformPostAsync(List<Parameter> parameters)
    {
        var endpoint = parameters.FirstOrDefault(p => p.Name == "endpoint").Value;
        var templateVariable = parameters.FirstOrDefault(p => p.Name == "body").Value;
        var templateName = TemplateReader.GetTemplateName(templateVariable);

        var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);

        return await _httpClient.PostAsync(endpoint, templateValue);
    }
    
    private async Task<Trace> PerformPutAsync(List<Parameter> parameters)
    {
        var endpoint = parameters.FirstOrDefault(p => p.Name == "endpoint").Value;
        var templateVariable = parameters.FirstOrDefault(p => p.Name == "body").Value;
        var templateName = TemplateReader.GetTemplateName(templateVariable);

        var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);

        return await _httpClient.PutAsync(endpoint, templateValue);
    }
    
    private async Task<Trace> PerformDeleteAsync(List<Parameter> parameters)
    {
        var endpoint = parameters.FirstOrDefault(p => p.Name == "endpoint").Value;
        
        return await _httpClient.DeleteAsync(endpoint);
    }

}