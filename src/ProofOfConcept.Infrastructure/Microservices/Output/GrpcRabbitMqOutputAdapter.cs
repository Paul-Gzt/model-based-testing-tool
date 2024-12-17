using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProofOfConcept.Core.Specifications;
using Dynamos.Grpc;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace ProofOfConcept.Infrastructure.Microservices.Output;

public class DynamosGrpcOutputAdapter : OutputAdapterBase
{
    private Dynamos.Grpc.RabbitMQ.RabbitMQClient _c;
    private AsyncServerStreamingCall<Dynamos.Grpc.SideCarMessage> _stream;

    public DynamosGrpcOutputAdapter()
    {
	var conn = GrpcChannel.ForAddress("http://localhost:50051");
	_c = new Dynamos.Grpc.RabbitMQ.RabbitMQClient(conn);
	_c.InitRabbitMq(new Dynamos.Grpc.InitRequest { ServiceName="tester-in", RoutingKey="tester-in", QueueAutoDelete=false });
	_stream = _c.Consume(new Dynamos.Grpc.ConsumeRequest { QueueName="tester-in", AutoAck=true });
   	   
    }

    public override void ReceiveResponse(Trace? trace)
    {
        throw new NotImplementedException();
    }

    protected override List<Trace> Observe()
    {
        return GetMessages(TimeSpan.FromSeconds(60));
    }
    
    private List<Trace> GetMessages(TimeSpan timeToRead)
    {
        var traces = new List<Trace>();
        try
        {
	   CancellationTokenSource source = new CancellationTokenSource();
	   CancellationToken token = source.Token;
	   source.CancelAfter(timeToRead);
	   Task<bool> result = _stream.ResponseStream.MoveNext(token);

           //Thread.Sleep(timeToRead);

	   if (result.Result)
	   {
	   	var current = _stream.ResponseStream.Current;
		var registry = TypeRegistry.FromMessages(Dynamos.Grpc.MicroserviceCommunication.Descriptor,
							 Dynamos.Grpc.SqlDataRequest.Descriptor);
		var formatter = new JsonFormatter(new JsonFormatter.Settings(false, registry));

		var jsonstring = formatter.Format(current.Body);
	
	
		traces.Add(new Trace(new Gate(ActionType.Output, "!grpc_" + current.Type), jsonstring));
	   }
	   
           return traces;
        }
	catch (AggregateException ae)
	{
	    ae.Handle(ex => { if (ex is RpcException) {
			    	 Console.WriteLine("RpcException caught: {0}", ex.Message);
				}
				return ex is RpcException;
			     });

	    return new List<Trace>();
	}
        catch (Exception e)
        {
            Console.WriteLine("Error while fetching GRPC messages: {0}", e);
            return new List<Trace>();
        }
    }

}
