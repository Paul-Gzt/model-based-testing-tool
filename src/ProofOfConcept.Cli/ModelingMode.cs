namespace ProofOfConcept.Cli;

public static class ModelingMode
{
    public static async Task<int> ExecuteAsync(string modelPath)
    {
        StartWatching(modelPath);

        Console.WriteLine("Press \'q\' to stop watching.\nPress \'t\' to perform tests with this model.");
        Console.WriteLine();

        var key = Console.ReadKey().Key;
        while (key != ConsoleKey.Q)
        {
            if (key == ConsoleKey.T)
            {
                Console.WriteLine("\nCommencing test with this model...");
                await ModelTestingMode.ExecuteAsync(modelPath);
            }

            key = Console.ReadKey().Key;
        }

        return 0;
    }
    
    private static void StartWatching(string modelPath)
    {
        var absolutePath = Path.IsPathFullyQualified(modelPath) ? modelPath : Path.GetFullPath(modelPath);
        
        var uri = new Uri(absolutePath);

        if (!uri.IsFile)
        {
            Console.WriteLine($"Path '{uri}' does not point to a file.");
            return;
        }

        var directory = Path.GetDirectoryName(uri.LocalPath) ?? "";
        var fileName = Path.GetFileName(uri.LocalPath);

        var watcher = new FileSystemWatcher();
        watcher.Path = directory;
        watcher.Filter = fileName;
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += ModelFileWatcher.OnChanged;
        watcher.EnableRaisingEvents = true;
    }
}