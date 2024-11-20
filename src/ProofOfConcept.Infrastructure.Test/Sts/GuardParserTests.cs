using FluentAssertions;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Sts;

namespace ProofOfConcept.Infrastructure.Test.Sts;

[TestClass]
public class GuardParserTests
{
    [TestMethod]
    public void ParseGuard_Null_ReturnsNothing()
    {
        // Arrange
        string? input = null;

        // Act
        var result = GuardParser.ParseGuard(input);

        // Assert
        result.Should().BeEmpty();
    }
    
    [TestMethod]
    public void ParseGuard_OneGuard_ReturnsThatGuard()
    {
        // Arrange
        string input = "endpoint == GetBasket";
        var expected = new List<Guard>()
        {
            new("endpoint", Operation.Equals, "GetBasket")
        };

        // Act
        var result = GuardParser.ParseGuard(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [TestMethod]
    public void ParseGuard_TwoGuards_ReturnsThatGuard()
    {
        // Arrange
        string input = "endpoint == GetBasket && body == t::GetRequestBody";
        var expected = new List<Guard>()
        {
            new("endpoint", Operation.Equals, "GetBasket"),
            new("body", Operation.Equals, "t::GetRequestBody")
        };

        // Act
        var result = GuardParser.ParseGuard(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}