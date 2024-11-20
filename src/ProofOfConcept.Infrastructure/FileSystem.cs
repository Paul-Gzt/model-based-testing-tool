namespace ProofOfConcept.Infrastructure;

public static class FileSystem
{
    public static void SaveFile(Stream fileStream, string fileName, string directoryName = "files")
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryName);
        var filePath = Path.Combine(path, fileName);
        Directory.CreateDirectory(path);
        using var file = File.Create(filePath);
        fileStream.CopyTo(file);
    }
}