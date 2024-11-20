using ProofOfConcept.Core.Specifications;
using RestSharp;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Http;

// TODO: Inject Singleton HttpClient using BaseUrl of model
public class HttpClient : IHttpClient
{
    public async Task<Trace> GetAsync(string endpoint)
    {
        using var restClient = new RestClient(endpoint);
        var request = new RestRequest();
        
        var result = await restClient.ExecuteGetAsync(request);

        return LabelMapping.MapToTrace(result);
    }

    public async Task<Trace> PostAsync(string endpoint, string body)
    {
        using var restClient = new RestClient(endpoint);
        var request = new RestRequest().AddJsonBody(body);

        var result = await restClient.ExecutePostAsync(request);

        return LabelMapping.MapToTrace(result);
    }

    public async Task<Trace> PutAsync(string endpoint, string body)
    {
        using var restClient = new RestClient(endpoint);
        var request = new RestRequest().AddJsonBody(body);

        var result = await restClient.ExecutePutAsync(request);

        return LabelMapping.MapToTrace(result);
    }

    public async Task<Trace> DeleteAsync(string endpoint)
    {
        using var restClient = new RestClient(endpoint);
        var request = new RestRequest();
        
        var result = await restClient.DeleteAsync(request);

        return LabelMapping.MapToTrace(result);
    }
}