using ProofOfConcept.Infrastructure.Sts.Subparts;

namespace ProofOfConcept.Infrastructure.Sts.CompletionAlgorithms;

public class CompleteWithAsynchronousOutputActions : ICompletionAlgorithm
{
    public ParsedSpecification PerformCompletion(ParsedSpecification result)
    {
        return HandleAsynchronousSwitches(result);
    }
    
    /// <summary>
    /// In order to support asynchronous traces, we can complete the specification with non-determnistic choices for each
    /// asynchronous output action. 
    /// </summary>
    /// <param name="parsedSpecification"></param>
    /// <returns></returns>
    private static ParsedSpecification HandleAsynchronousSwitches(ParsedSpecification parsedSpecification)
    {
        var asyncOutputSwitches = parsedSpecification.Switches.Where(s => s.Label.StartsWith("!") && s.IsAsync).ToList();
        if (!asyncOutputSwitches.Any()) return parsedSpecification;
        
        var asyncOutputActionChains = GetAsyncOutputActionChains(parsedSpecification.Switches);
        
        var result = CompleteSwitchesWithNonDeterministicPaths(parsedSpecification, asyncOutputActionChains);

        return parsedSpecification with {Switches = result};
    }

    // TODO: Somethings goes horribly wrong here, with a large number of permutations, we create a very large collection
    // of seemingly identical transitions
    /// <summary>
    /// For each output action chain, get its permutations and create a new chain of output actions with it.
    /// This completion assumes its own names, starting with '@'.
    /// 
    /// The last action of the chain should synchronize with its first following input action.
    /// </summary>
    /// <param name="parsedSpecification"></param>
    /// <param name="asyncOutputActionChains"></param>
    /// <returns></returns>
    private static List<Switch> CompleteSwitchesWithNonDeterministicPaths(ParsedSpecification parsedSpecification, List<List<Switch>> asyncOutputActionChains)
    {
        var result = new List<Switch>(parsedSpecification.Switches);
        
        var currentLocationIndex = 0;
        foreach (var asyncOutputActionChain in asyncOutputActionChains.Where(a => a.Any()))
        {
            var switchesToAdd = new List<Switch>();

            var startLocation = asyncOutputActionChain.First().From;
            var endLocation = asyncOutputActionChain.Last().To;
            var permutationsWithoutOriginalList =
                GetPermutations(asyncOutputActionChain, asyncOutputActionChain.Count)
                    .Where(x => !x.SequenceEqual(asyncOutputActionChain))
                    .ToList();
            
            var current = startLocation;
            var next = $"@l{currentLocationIndex}";
            foreach (var permutation in permutationsWithoutOriginalList.Select(x => x.ToList()))
            {
                var lastSwitch = permutation.Last();
                foreach (var permutationSwitch in permutation)
                {
                    if (permutationSwitch.Equals(lastSwitch))
                    {
                        switchesToAdd.Add(permutationSwitch with {From = current, To = endLocation});
                        continue;
                    }

                    var newSwitch = permutationSwitch with {From = current, To = next};
                    switchesToAdd.Add(newSwitch);
                    current = $"@l{currentLocationIndex++}";
                    next = $"@l{currentLocationIndex}";
                }

                current = startLocation;
            }

            result.AddRange(switchesToAdd);
        }

        return result;
    }

    /// <summary>
    /// An async output action chain is a sequence of asynchronous output actions without any input actions in between.
    /// Example: ?a, [!@b, !@c], ?d
    /// Example: ?a, [!@b], ?d
    /// Example: ?a, [!@b, !@c], ?d, [!@e, !@f]
    /// </summary>
    /// <param name="switches"></param>
    /// <returns></returns>
    private static List<List<Switch>> GetAsyncOutputActionChains(List<Switch> switches)
    {
        var asyncOutputActionChains = new List<List<Switch>>();

        var shouldStartAddingOutputActions = false;
        var currentList = new List<Switch>();
        foreach (var @switch in switches)
        {
            if (@switch.Label.IsInputLabel())
            {
                if (shouldStartAddingOutputActions)
                {
                    // Persist current results and reset
                    if (currentList.Any())
                    {
                        asyncOutputActionChains.Add(currentList);
                    }
                    currentList = new List<Switch>();
                    shouldStartAddingOutputActions = false;
                    continue;
                }

                shouldStartAddingOutputActions = true;
                continue;
            }

            if (!@switch.IsAsync || !@switch.Label.IsOutputLabel()) continue;
            
            currentList.Add(@switch);
        }
        
        asyncOutputActionChains.Add(currentList);

        return asyncOutputActionChains;
    }
    
    /// <summary>
    /// Gets all permutations of an IEnumerable
    /// </summary>
    /// <param name="list">The input list to create permutations on</param>
    /// <param name="length">The number of permutations to create</param>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <returns>A list containing lists of permutations. Includes the original list.</returns>
    private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });

        var enumerable = list.ToList();
        return GetPermutations(enumerable, length - 1)
            .SelectMany(t => enumerable.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }
}