using ProofOfConcept.Infrastructure;
using ProofOfConcept.Infrastructure.Utility;

namespace ProofOfConcept.Cli;

// Workaround to handle duplicate event handling, as this method might be called multiple times when we save and write our dot-file.
public static class ModelFileWatcher
{
    private static DateTime _lastRead = DateTime.MinValue;

    public static void OnChanged(object _, FileSystemEventArgs e)
    {
        var lastWriteTime = File.GetLastWriteTime(e.FullPath);
        if (lastWriteTime == _lastRead) return;

        try
        {
            var specification = ModelReader.ReadFromFileAsync(e.FullPath).GetAwaiter().GetResult();
            using var modelFile = GraphVizVisualizer.Visualize(specification);
            SaveModelAsync(modelFile, e.FullPath).GetAwaiter().GetResult();
            _lastRead = lastWriteTime;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static async Task SaveModelAsync(Stream modelFile, string modelFilePath)
    {
        var directory = Path.GetDirectoryName(modelFilePath) ?? "";
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modelFilePath);

        var path = Path.Combine(directory, $"{fileNameWithoutExtension}.dot");

        await using var fileStream = File.Create(path);
        modelFile.Seek(0, SeekOrigin.Begin);
        await modelFile.CopyToAsync(fileStream);

        Console.WriteLine($"Done writing file at: '{path}'");
    }
}