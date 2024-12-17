using System.Net;
using ProofOfConcept.Core.Specifications;
using RestSharp;

namespace ProofOfConcept.Infrastructure.Microservices.Protocol.Http;

public static class LabelMapping
{
    public static Dictionary<HttpStatusCode, string> SupportedLabels { get; } = new()
    {
        { HttpStatusCode.OK, "!http_ok" },
        { HttpStatusCode.Accepted, "!http_ok" },
        { HttpStatusCode.BadRequest, "!http_bad_request" },
        { HttpStatusCode.NotFound, "!http_not_found" },
        { HttpStatusCode.InternalServerError, "!http_internal_server_error" },
	{ HttpStatusCode.Unauthorized, "!http_unauthorized" },
	{ HttpStatusCode.BadGateway, "!http_bad_gateway" },
    };

    public static Trace MapToTrace(RestResponseBase restResponse)
    {
        if (!SupportedLabels.TryGetValue(restResponse.StatusCode, out var label) && label is null)
        {
	    label = "!http_unknown";
        //    throw new NotImplementedException($"Unsupported StatusCode: {restResponse.StatusCode}");
        }

        return new Trace(new Gate(ActionType.Output, label), restResponse.Content ?? "");
    }
}
