using ProofOfConcept.Infrastructure.Sts.Subparts;
using Sprache;

namespace ProofOfConcept.Infrastructure.Sts.CompletionAlgorithms;

public class CompleteWithDefaultLocations : ICompletionAlgorithm
{
    private const string LocationPrefix = "l";
    
    /// <summary>
    /// Our modeling formalism allows for the underspecification of location names.
    /// If any location name is underspecified, we will transform every switch name.
    /// TODO: We do however, want to support non-deterministic choices. How to do that?
    /// This completion algorithm will do the following:
    /// - Assume the first Switch of the model is the starting state
    /// - Add default names (l0, l1, .... ln) for each switch statement it encounters
    /// </summary>
    public ParsedSpecification PerformCompletion(ParsedSpecification result)
    {
        if (result.Switches.All(x => !string.IsNullOrEmpty(x.From) || !string.IsNullOrEmpty(x.To))) return result;
        if (result.Switches.Any(x => !string.IsNullOrEmpty(x.From) || !string.IsNullOrEmpty(x.To)))
        {
            throw new ParseException("Tried to automatically infer location names, but some locations have defined names already." +
                                     "This could indicate an invalid specification");
        }
        
        result = result with {Switches = AddLocationNames(result.Switches)};

        if (result.InitialLocation is null)
        {
            result = result with {InitialLocation = result.Switches.First().From};
        }

        return result;
    }
    
    private static List<Switch> AddLocationNames(List<Switch> inputSwitches)
    {
        var index = 0;

        return inputSwitches.Select(inputSwitch => inputSwitch with {From = $"{LocationPrefix}{index++}", To = $"{LocationPrefix}{index}"}).ToList();
    }
}