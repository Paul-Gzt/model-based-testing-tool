namespace ProofOfConcept.Infrastructure.Test;

public static class FileHelper
{
    public static async Task<string> ReadFileAsync(string directory, string fileName)
    {
        var fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory, fileName);
        return await File.ReadAllTextAsync(fileLocation);
    }
}