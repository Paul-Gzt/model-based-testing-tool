using FluentAssertions;
using ProofOfConcept.Infrastructure.Utility;

namespace ProofOfConcept.Infrastructure.Test;

[TestClass]
public class GraphVizVisualizerTests
{
    [TestMethod]
    public async Task TestMethod1()
    {
        // Arrange
        var specification = await ModelReader.ReadFromFileAsync("Models", "SimpleModel.model");

        // Act
        var actualStream = GraphVizVisualizer.Visualize(specification);
        using var reader = new StreamReader(actualStream);

        var actualFileLines = await reader.ReadToEndAsync();
        
        // Assert
        actualFileLines.Should().Be(@"digraph G {
init [xlabel=""{
x: 0
}
"", shape=point]
""init"" -> ""l0""
""l0"" -> ""l1"" [""label"" = ""?inX\n x == 0\n x = x + 1""]
""l1"" -> ""l2"" [""label"" = ""!outX\n x == 1\n ""]
}");   
    }
}