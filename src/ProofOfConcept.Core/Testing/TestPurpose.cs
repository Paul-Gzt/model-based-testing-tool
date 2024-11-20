using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Core.Testing;

public class TestPurpose
{
    private readonly Queue<Switch> _testPath;

    public TestPurpose(Queue<Switch> testPath)
    {
        _testPath = testPath;
    }

    public bool HasNext => _testPath.Count != 0;

    public Switch GetNextAction()
    {
        return _testPath.Dequeue();
    }

    /// <summary>
    /// Assumes non-deterministic output actions to have the same number of locations
    /// in each path.
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public static TestPurpose GenerateFrom(Specification specification)
    {
        var queue = new Queue<Switch>();
        
        foreach (var inputAction in specification.Switches
                     .Where(s => s.Gate.ActionType == ActionType.Input))
        {
            queue.Enqueue(inputAction);

            var outputActionChain = inputAction.GetOutputActionChains(specification);

            outputActionChain.ForEach(o => queue.Enqueue(o));
        }

        return new TestPurpose(queue);
    }
}