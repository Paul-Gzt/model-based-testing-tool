using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace ProofOfConcept.Cli;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var modelPath = new Argument<string>("model-path", "The file path of the model file.");
        var modelingMode = new Option<bool>("--modeling-mode", "Creates a new model on every file change. Does not run any tests.");

        var cmd = new RootCommand { modelPath, modelingMode};
        cmd.Handler = CommandHandler.Create(ActAsync);

        return await cmd.InvokeAsync(args);
    }

    private static async Task<int> ActAsync(string modelPath, bool modelingMode)
    {
        try
        {
            if (modelingMode)
            {
                return await ModelingMode.ExecuteAsync(modelPath);
            }

            return await ModelTestingMode.ExecuteAsync(modelPath);
        }
        catch (Exception e)
        {
            Console.WriteLine("An unrecoverable error has occurred. Exiting.");
            Console.WriteLine("Error message: '{0}'", e);
            return 1;
        }
    }
}
