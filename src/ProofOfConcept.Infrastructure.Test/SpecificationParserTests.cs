using FluentAssertions;
using ProofOfConcept.Infrastructure.Sts;
using Sprache;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class SpecificationParserTests
{
    
    [DataRow("  baseUrl:" +
             "    ")]
    [DataRow("     baseUrl:     ")]
    [DataRow("    baseUrl:")]
    [DataRow("baseUrl:    ")]
    [DataRow("baseUrl:")]
    [DataRow("globalVariables:")]
    [DataRow("endpoints:")]
    [DataRow("switches:")]
    [DataRow("switches:blablabla")]
    [DataTestMethod]
    public void Parses_Identifier_Successfully(string input)
    {
        // Arrange
        var expected = input.Split(":").First().Trim();

        // Act
        var actual = SpecificationParser.Identifier.Parse(input);
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Value.Should().Be(expected);
    }
    
    [DataRow(@"""someValue""")]
    [DataRow(@"""""")]
    [DataTestMethod]
    public void Parses_QuotedText_Successfully(string input)
    {
        // Arrange
        var expected = input.Split("\"")[1];

        // Act
        var actual = SpecificationParser.QuotedText.Parse(input);
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Should().Be(expected);
    }
    
    [DataRow(@"identifier: ""someValue""", "identifier","someValue")]
    [DataRow(@"value: ""123""", "value" ,"123")]
    [DataRow(@"value: ""value""", "value" ,"value")]
    [DataRow(@"     value:     ""   value""", "value" , "   value")]
    [DataTestMethod]
    public void Parses_Section_Successfully(string input, string expectedIdentifier, string expectedValue)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.Section.Parse(input);
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Identifier.Should().Be(expectedIdentifier);
        actual!.Value.Should().Be(expectedValue);
    }
    
    [DataRow(@"[1,2,3,4,5,6,7,8]", "1,2,3,4,5,6,7,8")]
    [DataRow(@"[1]", "1")]
    [DataRow(@"[someValue, url, 123]", "someValue, url, 123")]
    [DataRow(@"[]", "")]
    [DataTestMethod]
    public void Parses_Array_Successfully(string input, string expected)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.Array.Parse(input);
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Should().Be(expected);
    }
    
    [DataRow(@"values: [1,2,3,4]", "values" , new[] { "1", "2", "3", "4" })]
        [DataRow(@"values: [1]", "values" , new[] { "1" })]
    [DataRow(@"values: [blaaaaa]", "values" , new[] { "blaaaaa" })]
    [DataRow(@"whitespace: [1,   2,  3,4]", "whitespace" ,  new[] { "1", "2", "3", "4" })]
    [DataRow(@"whitespace: [1,
2,
3]", "whitespace" , new[] { "1", "2", "3" })]
    [DataRow(@"values: []", "values" ,  new string[0])]
    [DataTestMethod]
    public void Parses_SectionWithList_Successfully(string input, string expectedIdentifier, string[] expectedValue)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.SectionWithList.Parse(input);
        
        // Assert
        actual.Should().NotBeNull();
        actual!.Identifier.Should().Be(expectedIdentifier);
        actual!.Values.Should().BeEquivalentTo(expectedValue);
    }

    [DataRow("l1 !input(x) l2 where x > 1 update global_x = x + 1\n", "l1", "!input(x)", "l2", "x > 1", "global_x = x + 1")]
    [DataRow("l1 !input(x) l2 where x > 1\n", "l1", "!input(x)", "l2", "x > 1", null)]
    [DataRow("l1 !input(x) l2\n", "l1", "!input(x)", "l2", null, null)]
    [DataRow("l1 !input(x) l2 update global_x = x + 1\n", "l1", "!input(x)", "l2", null, "global_x = x + 1")]
    [DataRow("!input(x) where x > 1 update global_x = x + 1\n", null, "!input(x)", null, "x > 1", "global_x = x + 1")]
    [DataRow("!input(x)\n", null, "!input(x)", null, null, null)]
    [DataTestMethod]
    public void Parses_Switch(string input, string from, string label, string to, string where, string updateMapping)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.Switch.Parse(input);

        // Assert
        actual.Should().NotBeNull();
        actual.From.Should().Be(from);
        actual.Label.Should().Be(label);
        actual.To.Should().Be(to);
        actual.WhereClause.Should().Be(where);
        actual.UpdateMapping.Should().Be(updateMapping);
    }
    
    // TODO: We can't parse variables that contain keywords: update, where
    [Ignore]
    [DataRow("!input(x) where x == 0 || x == updateBody\n", null, "!input(x)", null, "x == 0 || x == 0", null)]
    [DataTestMethod]
    public void Parses_Switch_Ignored(string input, string from, string label, string to, string where, string updateMapping)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.Switch.Parse(input);

        // Assert
        actual.Should().NotBeNull();
        actual.From.Should().Be(from);
        actual.Label.Should().Be(label);
        actual.To.Should().Be(to);
        actual.WhereClause.Should().Be(where);
        actual.UpdateMapping.Should().Be(updateMapping);
    }
    
    [DataRow("?input(x)\n!output(x)\n!done\n", 3 ,new [] { "?input(x)", "!output(x)", "!done" })]
    [DataTestMethod]
    public void Parses_Switches(string input, int count, string[] expectedLabels)
    {
        // Arrange - DataRow

        // Act
        var actual = SpecificationParser.Switches.Parse(input);

        // Assert
        actual.Should().NotBeNull();
        actual.Count.Should().Be(count);
        actual.Select(x => x.Label).Should().BeEquivalentTo(expectedLabels);
    }
}