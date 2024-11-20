using FluentAssertions;
using Moq;
using ProofOfConcept.Core.Assertion;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing;
using ProofOfConcept.Infrastructure.Utility;
using TestResult = ProofOfConcept.Core.Testing.TestResult;

namespace ProofOfConcept.Core.Test;

[TestClass]
public class Algorithm1Tests
{
    private Mock<IInputGenerator> _inputGeneratorMock = null!;
    private Mock<IAsserter> _asserterMock = null!;

    private Algorithm1 _sut = null!;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _inputGeneratorMock = new Mock<IInputGenerator>();
        _asserterMock = new Mock<IAsserter>();

        _sut = new Algorithm1(_inputGeneratorMock.Object);
    }
    
    [TestMethod]
    public async Task Test()
    {
        // Arrange
        var specification = new Specification
        {
            InitialLocation = new("l1"),
            Locations = new List<Location> { new("l1"), new("l2") },
            Gates = new List<Gate> { new(ActionType.Input, "?but") },
            Switches = new List<Switch>
            {
                new(
                    new Location("l1"), 
                    new Gate(ActionType.Input, "?but"),
                    new List<Guard>(),
                    new UpdateMapping(),
                    new Location("l2")
                )
            }
        };

        var specificationUnderTest = new SpecificationUnderTest(specification, 
            new SpecificationState
            {
                InstantiatedLocations = new List<Location> { new("l1") },
                InteractionVariables = new List<Variable>(),
                LocationVariables = new List<Variable>()
            }, _asserterMock.Object);
        
        var testPath = new Queue<Switch>();
        testPath.Enqueue(specification.Switches.First());
        var testPurpose = new TestPurpose(testPath);

        _inputGeneratorMock
            .Setup(x => x.GenerateValue(It.IsAny<List<Guard>>(), It.IsAny<List<Variable>>()))
            .Returns(new List<Parameter>());
        
        var systemUnderTestMock = new Mock<ISystemUnderTest>();
        
        // Act
        var actual = await _sut.ExecuteAsync(specificationUnderTest, testPurpose, systemUnderTestMock.Object);

        // Assert
        actual.Should().Be(new TestResult(TestVerdict.Passed, string.Empty));
        systemUnderTestMock.Verify(x => x.SendInput("?but", new List<Parameter>()));
    }

    [TestMethod]
    public async Task TestAlgorithm_SutDisplaysExpectedBehavior_PassesTest()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "SegTest1.model");
        var testPath = new Queue<Switch>();
        specification.Switches.ForEach(x => testPath.Enqueue(x));
        var testPurpose = new TestPurpose(testPath);
        var specificationUnderTest = new SpecificationUnderTest(specification, specification.CurrentState, _asserterMock.Object);
        
        _inputGeneratorMock
            .Setup(x => x.GenerateValue(It.IsAny<List<Guard>>(), It.IsAny<List<Variable>>()))
            .Returns(new List<Parameter>
            {
                new("p", "1")
            });
        _asserterMock.Setup(x => x.AssertAsync("1", It.IsAny<Switch>())).ReturnsAsync(new AssertionResult(true, string.Empty));

        var systemUnderTestMock = new Mock<ISystemUnderTest>();
        systemUnderTestMock.Setup(x => x.ReceiveOutput()).Returns(new Trace(new Gate(ActionType.Output, "!outX"), "1"));
        
        // Act
        var actual = await _sut.ExecuteAsync(specificationUnderTest, testPurpose, systemUnderTestMock.Object);

        // Assert
        actual.Should().Be(new TestResult(TestVerdict.Passed, string.Empty));
        systemUnderTestMock.Verify(x => x.SendInput("?inX", new List<Parameter>
        {
            new("p", "1")
        }));
    }
    
    [TestMethod]
    public async Task TestAlgorithm_SutDisplaysExpectedBehavior_NonDeterministic_PassesTest()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "NonDeterministicChoices.model");
        var testPurpose = TestPurpose.GenerateFrom(specification);
        var specificationUnderTest = new SpecificationUnderTest(specification, specification.CurrentState, _asserterMock.Object);

        _inputGeneratorMock
            .Setup(x => x.GenerateValue(It.IsAny<List<Guard>>(), It.IsAny<List<Variable>>()))
            .Returns(new List<Parameter>());
        _asserterMock.Setup(x => x.AssertAsync("", It.IsAny<Switch>())).ReturnsAsync(new AssertionResult(true, string.Empty));

        var systemUnderTestMock = new Mock<ISystemUnderTest>();
        systemUnderTestMock.Setup(x => x.ReceiveOutput()).Returns(new Trace(new Gate(ActionType.Output, "!output1"), ""));
        
        // Act
        var actual = await _sut.ExecuteAsync(specificationUnderTest, testPurpose, systemUnderTestMock.Object);

        // Assert
        actual.Should().Be(new TestResult(TestVerdict.Passed, string.Empty));
    }
    
    [TestMethod]
    public async Task TestAlgorithm_TemplateAssumesResponseData_AndSutRespondsWithCorrectLabel_ButWrongData_Fails()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "HttpTest.model");
        var testPurpose = TestPurpose.GenerateFrom(specification);
        var specificationUnderTest = new SpecificationUnderTest(specification, specification.CurrentState, _asserterMock.Object);

        _inputGeneratorMock
            .Setup(x => x.GenerateValue(It.IsAny<List<Guard>>(), It.IsAny<List<Variable>>()))
            .Returns(new List<Parameter>
            {
                new("endpoint", "http://localhost/v1/api")
            });
        _asserterMock.Setup(x => x.AssertAsync(It.IsAny<string>(), It.IsAny<Switch>())).ReturnsAsync(new AssertionResult(false, string.Empty));

        var systemUnderTestMock = new Mock<ISystemUnderTest>();
        systemUnderTestMock
            .SetupSequence(x => x.ReceiveOutput())
            .Returns(new Trace(new Gate(ActionType.Output, "!http_ok"), string.Empty))
            .Returns(Trace.Empty);
        
        // Act
        var actual = await _sut.ExecuteAsync(specificationUnderTest, testPurpose, systemUnderTestMock.Object);

        // Assert
        actual.TestVerdict.Should().Be(TestVerdict.Failed);
        actual.Reason.Should().Be("'!http_ok' did not match any expected values: ");
        systemUnderTestMock.Verify(x => x.SendInput("?http_get", new List<Parameter>
        {
            new("endpoint", "http://localhost/v1/api")
        }));
        _asserterMock.Verify(x => x.AssertAsync(It.IsAny<string>(), It.IsAny<Switch>()));
    }
}