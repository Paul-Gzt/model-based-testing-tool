using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using Dynamos.Grpc;
using ProofOfConcept.Infrastructure.Templates;
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public class DynamosGrpcInputAdapter : InputAdapterBase
{
    private static readonly List<string> SupportedLabels = new()
    {
        "?grpc_init_rabbitmq",
	"?grpc_request_approval",
	"?grpc_sqlDataRequest",
	"?grpc_compositionRequest",
	"?grpc_microserviceCommunication",
	"?grpc_stop_request",
	"?grpc"
    };

    private Dynamos.Grpc.RabbitMQ.RabbitMQClient _c;
    private readonly TestingContext _testingContext;
    
    public DynamosGrpcInputAdapter(TestingContext testingContext) : base(SupportedLabels)
    {
        _testingContext = testingContext;

	var conn = GrpcChannel.ForAddress("http://localhost:50051");
	_c = new Dynamos.Grpc.RabbitMQ.RabbitMQClient(conn);
    }

    protected override async Task<Trace?> DoAction(string label, List<Parameter> parameters)
    {
        return label switch
        {
            "?grpc_init_rabbitmq" => await InitReq(parameters),
	    "?grpc_request_approval" => await ReqApproval(parameters),
	    "?grpc_sqlDataRequest" => await SqlDataReq(parameters),
	    "?grpc_compositionRequest" => await CompReq(parameters),
	    "?grpc_microserviceCommunication" => await MicroserviceComm(parameters),
	    "?grpc_stop_request" => await StopReq(parameters),
	    "?grpc" => await HandleGrpc(parameters),
            _ => throw new NotImplementedException($"Unsupported label: {label}")
        };
    }

    private async Task<Trace?> HandleGrpc(List<Parameter> parameters)
    {
	var body = parameters.FirstOrDefault(p => p.Name == "body").Value;
	var templateName = TemplateReader.GetTemplateName(body);
	var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);
       
	var rpc = parameters.FirstOrDefault(p => p.Name == "rpc").Value.Replace("\"", string.Empty);
	
	if (rpc == "SendSqlDataRequest") {
		var request = JsonParser.Default.Parse<Dynamos.Grpc.SqlDataRequest>(templateValue);
		await Task.Run(() => _c.SendSqlDataRequest(request));
	} else if (rpc == "SendCompositionRequest") {
		var request = JsonParser.Default.Parse<Dynamos.Grpc.CompositionRequest>(templateValue);
		await Task.Run(() => _c.SendCompositionRequest(request));
	} else if (rpc == "SendMicroserviceComm") {
		var registry = TypeRegistry.FromMessages(Dynamos.Grpc.SqlDataRequest.Descriptor);
		var parser = new JsonParser(new JsonParser.Settings(10, registry));
		var request = parser.Parse<Dynamos.Grpc.MicroserviceCommunication>(templateValue);
		await Task.Run(() => _c.SendMicroserviceComm(request));
	}	
	return null;
    }
	
    private async Task<Trace?> InitReq(List<Parameter> parameters)
    {
        var serviceName = parameters.FirstOrDefault(p => p.Name == "serviceName").Value.Replace("\"", string.Empty);
        var routingKey = parameters.FirstOrDefault(p => p.Name == "routingKey").Value.Replace("\"", string.Empty);
        var queueAutoDelete = Convert.ToBoolean(parameters.FirstOrDefault(p => p.Name == "queueAutoDelete").Value.Replace("\"", string.Empty));

    	await Task.Run(() => _c.InitRabbitMq(new Dynamos.Grpc.InitRequest { ServiceName=serviceName, RoutingKey=routingKey, QueueAutoDelete=queueAutoDelete }));

	return null;
    }
    
    private async Task<Trace?> StopReq(List<Parameter> parameters)
    {
    	await Task.Run(() => _c.StopReceivingRabbit(new Dynamos.Grpc.StopRequest { }));

	return null;
    }

    private async Task<Trace?> ReqApproval(List<Parameter> parameters)
    {
        var type = parameters.FirstOrDefault(p => p.Name == "type").Value.Replace("\"", string.Empty);
        var id = parameters.FirstOrDefault(p => p.Name == "id").Value.Replace("\"", string.Empty);
        var userName = parameters.FirstOrDefault(p => p.Name == "userName").Value.Replace("\"", string.Empty);
        var dataProvider = parameters.FirstOrDefault(p => p.Name == "dataProvider").Value.Replace("\"", string.Empty);
        var destinationQueue = parameters.FirstOrDefault(p => p.Name == "destinationQueue").Value.Replace("\"", string.Empty);
        
        await Task.Run(() => _c.SendRequestApproval(new Dynamos.Grpc.RequestApproval { Type = type,
						     User = new Dynamos.Grpc.User { Id=id, UserName=userName },
						     DataProviders = { dataProvider },
						     DestinationQueue = destinationQueue }));
        // This call does not produce a trace, since it is publish/subscribe
        return null;
    }
    
    private async Task<Trace?> SqlDataReq(List<Parameter> parameters)
    {
	var body = parameters.FirstOrDefault(p => p.Name == "body").Value;
	var templateName = TemplateReader.GetTemplateName(body);
	var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);
        
	var request = JsonParser.Default.Parse<Dynamos.Grpc.SqlDataRequest>(templateValue);

        await Task.Run(() => _c.SendSqlDataRequest(request));
        return null;
    }
    
    private async Task<Trace?> CompReq(List<Parameter> parameters)
    {
	var body = parameters.FirstOrDefault(p => p.Name == "body").Value;
	var templateName = TemplateReader.GetTemplateName(body);
	var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);
        
	var request = JsonParser.Default.Parse<Dynamos.Grpc.CompositionRequest>(templateValue);

        await Task.Run(() => _c.SendCompositionRequest(request));
        return null;
    }
    
    private async Task<Trace?> MicroserviceComm(List<Parameter> parameters)
    {
	var body = parameters.FirstOrDefault(p => p.Name == "body").Value;
	var templateName = TemplateReader.GetTemplateName(body);
	var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);
	
	var registry = TypeRegistry.FromMessages(Dynamos.Grpc.SqlDataRequest.Descriptor);
	var parser = new JsonParser(new JsonParser.Settings(10, registry));
	var request = parser.Parse<Dynamos.Grpc.MicroserviceCommunication>(templateValue);

        await Task.Run(() => _c.SendMicroserviceComm(request));
        return null;
    }

}
