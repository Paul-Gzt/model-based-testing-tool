using FluentAssertions;
using Moq;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Microservices;
using ProofOfConcept.Infrastructure.Microservices.Input;
using ProofOfConcept.Infrastructure.Microservices.Output;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class MicroserviceTests
{
    private Mock<IInputAdapter> _inputAdapterMock = null!;
    private Mock<IOutputAdapter> _outputAdapterMock = null!;

    private Microservice _sut = null!;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _inputAdapterMock = new Mock<IInputAdapter>();
        _outputAdapterMock = new Mock<IOutputAdapter>();

        _sut = new Microservice(new AdapterChain<IInputAdapter>(new List<IInputAdapter>
        {
            _inputAdapterMock.Object
        }), new AdapterChain<IOutputAdapter>(new List<IOutputAdapter>
        {
            _outputAdapterMock.Object
        }));
    }
    
    [TestMethod]
    public async Task SendInput_Calls_InputAdapter()
    {
        // Arrange
        var label = "!test";
        var parameters = new List<Parameter>();
        
        // Act
        await _sut.SendInput(label, parameters);

        // Assert
        _inputAdapterMock.Verify(x => x.PerformActionAsync(label, parameters));
    }
    
    [TestMethod]
    public void ReceiveOutput_SUT_ReturnsNoTrace_ReturnsNothing()
    {
        // Arrange
        _outputAdapterMock
            .Setup(x => x.ObserveOutput())
            .Returns(new List<Trace>());
        
        // Act
        var output = _sut.ReceiveOutput();

        // Assert
        output.Result.ActionType.Should().Be(ActionType.Unknown);
    }
    
    [TestMethod]
    public void ReceiveOutput_SUT_ReturnsOneTrace_ReturnsThatOneTrace_WithoutCaching()
    {
        // Arrange
        var trace = new Trace(new Gate(ActionType.Output, "!ok"), "");

        _outputAdapterMock
            .Setup(x => x.ObserveOutput())
            .Returns(new List<Trace> {trace});
        
        // Act
        var output = _sut.ReceiveOutput();

        // Assert
        output.Should().Be(trace);
    }
    
    [TestMethod]
    public void ReceiveOutput_SUT_ReturnsTwoTraces_ReturnsFirstTrace_AndCachesNext()
    {
        // Arrange
        var trace1 = new Trace(new Gate(ActionType.Output, "!ok"), "");
        var trace2 = new Trace(new Gate(ActionType.Output, "!publish"), "");

        _outputAdapterMock
            .Setup(x => x.ObserveOutput())
            .Returns(new List<Trace> {trace1, trace2});
        
        // Act
        var output1 = _sut.ReceiveOutput();

        // Assert
        output1.Should().Be(trace1);
        var output2 = _sut.ReceiveOutput();
        output2.Should().Be(trace2);
    }
}