using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Sts;

namespace ProofOfConcept.Infrastructure.Utility;

public static class ModelReader
{
    public static async Task<Specification> ReadFromFileAsync(string directory, string fileName)
    {
        var fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory, fileName);
        return await ReadFromFileAsync(fileLocation);
    }

    public static async Task<Specification> ReadFromFileAsync(string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var fileReader = new StreamReader(fileStream);

        var modelContents = await fileReader.ReadToEndAsync();
        
        return SpecificationParser.ParseSpecification(modelContents);
    }
}