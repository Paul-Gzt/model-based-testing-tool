using System.Text;
using ProofOfConcept.Core.Assertion;
using ProofOfConcept.Core.Solver;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Core.Testing.Reporting;

namespace ProofOfConcept.Core.Testing;

public class Algorithm1
{
    private readonly IInputGenerator _inputGenerator;

    public Algorithm1(IInputGenerator inputGenerator)
    {
        _inputGenerator = inputGenerator;
    }

    public async Task<TestResult> ExecuteAsync(SpecificationUnderTest specification, TestPurpose testPurpose, ISystemUnderTest sut)
    {
        while (testPurpose.HasNext)
        {
            var nextSwitch = testPurpose.GetNextAction();

            if (nextSwitch.Gate.Label == "!quiescence")
            {
                await HandleQuiescence(specification, nextSwitch);
                continue;
            }

            if (nextSwitch.Gate.ActionType == ActionType.Input)
            {
                await HandleInputAction(specification, sut, nextSwitch);
                continue;
            }

            var result = await HandleOutputAction(specification, sut, nextSwitch);

            if (result.TestVerdict == TestVerdict.Failed) return result;
        }
        
        return new TestResult(TestVerdict.Passed, string.Empty);
     }

    /// <summary>
    /// 1. Observe output from SUT
    /// 2. Check whether the output label can be used to perform a valid transition within specification
    /// 3. Take valid transitions and check if data is correct
    /// 4. If data is correct, continue testing. Otherwise we have found a diverging SUT.
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="sut"></param>
    /// <param name="nextSwitch"></param>
    /// <returns></returns>
    private async Task<TestResult> HandleOutputAction(SpecificationUnderTest specification, ISystemUnderTest sut, Switch nextSwitch)
    {
        Console.WriteLine("OUTPUT ({0})", nextSwitch.Gate.Label);
        var output = sut.ReceiveOutput();
        var result = await specification.SwitchOutput(output.Result, output.Value);

        TestReporter.ProcessOutput(new OutputTrace(nextSwitch.Gate.Label, output, result.Expected));
        
        return result.IsExpected ? 
            new TestResult(TestVerdict.Continue, "") :
            new TestResult(TestVerdict.Failed, FormatUnexpectedLabelMessage(output.Result, result.Expected));
    }

    private async Task HandleInputAction(SpecificationUnderTest specification, ISystemUnderTest sut, Switch nextSwitch)
    {
        Console.WriteLine("INPUT ({0})", nextSwitch.Gate.Label);
        var input = _inputGenerator.GenerateValue(nextSwitch.Guards, specification.CurrentState.LocationVariables);
        await sut.SendInput(nextSwitch.Gate.Label, input);
        await specification.SwitchInput(nextSwitch.Gate);
        TestReporter.ProcessInput(new InputTrace(nextSwitch.Gate.Label, input));
    }

    private static async Task HandleQuiescence(SpecificationUnderTest specification, Switch nextSwitch)
    {
        var timeoutAsString = nextSwitch.Guards.First(x => x.LeftOperand == "timeoutInMilliseconds").RightOperand;
        if (int.TryParse(timeoutAsString, out var timeout))
        {
            Console.WriteLine("Quiescence, waiting '{0}' milliseconds", timeout);
            await Task.Delay(timeout);
            await specification.SwitchInput(nextSwitch.Gate);
        }
    }

    // TODO: Shouldn't be here
    // TODO: ActionTypes
    // TODO: if unknown trace, still include expected transitions
    private static string FormatUnexpectedLabelMessage(Gate actual, List<Gate> expected)
    {
        if (actual.ActionType == ActionType.Unknown)
        {
            return "Unexpected output: observed nothing from SUT";
        }
        
        var stringBuilder = new StringBuilder($"'{actual.Label}' did not match any expected values: ");
        
        expected.ForEach(x => stringBuilder.AppendLine($"{x.Label}"));

        return stringBuilder.ToString();
    }
}