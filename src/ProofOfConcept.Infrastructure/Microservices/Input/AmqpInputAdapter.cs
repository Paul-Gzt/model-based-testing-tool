using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices.Protocol.Amqp;
using ProofOfConcept.Infrastructure.Templates;

namespace ProofOfConcept.Infrastructure.Microservices.Input;

public class AmqpInputAdapter : InputAdapterBase
{
    private static readonly List<string> SupportedLabels = new()
    {
        "?amqp_receive"
    };

    private readonly IMessagePublisher _messagePublisher;
    private readonly TestingContext _testingContext;
    
    public AmqpInputAdapter(IMessagePublisher messagePublisher, TestingContext testingContext) : base(SupportedLabels)
    {
        _messagePublisher = messagePublisher;
        _testingContext = testingContext;
    }

    protected override async Task<Trace?> DoAction(string label, List<Parameter> parameters)
    {
        return label switch
        {
            "?amqp_receive" => await PublishAmqpMessageAsync(parameters),
            _ => throw new NotImplementedException($"Unsupported label: {label}")
        };
    }

    private async Task<Trace?> PublishAmqpMessageAsync(List<Parameter> parameters)
    {
        var exchangeName = parameters.FirstOrDefault(p => p.Name == "exchangeName").Value.Replace("\"", string.Empty);
        var routingKey = parameters.FirstOrDefault(p => p.Name == "routingKey").Value.Replace("\"", string.Empty);
        var templateVariable = parameters.FirstOrDefault(p => p.Name == "body").Value;
        var templateName = TemplateReader.GetTemplateName(templateVariable);

        var templateValue = await TemplateReader.GetTemplateAsync(_testingContext.BaseDirectory, templateName);
        
        _messagePublisher.Publish(routingKey, exchangeName, templateValue);
        
        // This call does not produce a trace, since it is publish/subscribe
        return null;
    }
}