using System.Collections.Immutable;
using ProofOfConcept.Core.Specifications;
using ProofOfConcept.Infrastructure.Sts.CompletionAlgorithms;
using ProofOfConcept.Infrastructure.Sts.Subparts;
using Sprache;
using Switch = ProofOfConcept.Infrastructure.Sts.Subparts.Switch;
using Variable = ProofOfConcept.Infrastructure.Sts.Subparts.Variable;

namespace ProofOfConcept.Infrastructure.Sts;

public static class SpecificationParser
{
    private static readonly ICompletionAlgorithm CompleteDefaultLocations;
    private static readonly ICompletionAlgorithm CompleteAsynchronousOutputActions;

    static SpecificationParser()
    {
        CompleteDefaultLocations = new CompleteWithDefaultLocations();
        CompleteAsynchronousOutputActions = new CompleteWithAsynchronousOutputActions();
    }
    
    public static readonly Parser<Identifier> Identifier = Parse
        .Char(c => char.IsLetter(c) || char.IsWhiteSpace(c), "identifier")
        .Until(Parse.Char(':'))
        .Text()
        .Select(t => new Identifier(t.Trim()));

    public static readonly Parser<string> QuotedText =
        (from open in Parse.Char('"')
            from content in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"')
            select content).Token();

    public static readonly Parser<string> Array =
        (from open in Parse.Char('[')
            from content in Parse.Char(c => c != '[' && c != ']', "except array brackets").Many().Text()
            from close in Parse.Char(']')
            select content).Token();

    public static readonly Parser<Section> Section =
        from identifier in Identifier
        from value in QuotedText
        select new Section(identifier.Value, value);

    public static readonly Parser<SectionWithList> SectionWithList =
        from identifier in Identifier
        from values in Array.Select(x => x.Split(",").SkipWhile(string.IsNullOrEmpty).Select(s => s.Trim()).ToList())
        select new SectionWithList(identifier.Value, values);

    public static readonly Parser<string> Line = Parse
        .AnyChar
        .Until(Parse.LineTerminator)
        .Text();
    
    public static readonly Parser<Switch?> Switch =
        from line in Line
        select ParseSwitch(line);

    public static Switch? ParseSwitch(string line)
    {
        if (string.IsNullOrEmpty(line)) return null;
        var (whereClause, updateMapping, sectionWithLocationsAndLabel) = ParseExpressions(line);

        var (from, label, to) = ParseLocationsAndLabel(sectionWithLocationsAndLabel);

        return new Switch(from, label, to, whereClause?.Trim(), updateMapping?.Trim(), label.IsAsyncOutputLabel());
    }

    public static (string? WhereClause, string? UpdateMapping, string SectionWithLocationsAndLabel) ParseExpressions(string input)
    {
        var partitionedByWhereExpression = input.Split("where").ToList();
        
        switch (partitionedByWhereExpression.Count)
        {
            // If there exists no where clause, check for an update statement
            case 1:
            {
                var partitionedByUpdateExpression = input.Split("update").ToList();

                if (partitionedByUpdateExpression.Count == 2)
                {
                    return (null, partitionedByUpdateExpression.ElementAt(1), partitionedByUpdateExpression.ElementAt(0));
                }

                break;
            }
            // If there exists an expression, try to parse that 
            case 2:
            {
                var partAfterWhere = partitionedByWhereExpression.ElementAt(1);
                var partitionedByUpdate = partAfterWhere.Split("update").ToList();

                if (partitionedByUpdate.Count == 1)
                {
                    return (partAfterWhere, null, partitionedByWhereExpression.ElementAt(0));
                }

                return (partitionedByUpdate.ElementAt(0), partitionedByUpdate.ElementAt(1), partitionedByWhereExpression.ElementAt(0));
            }
        }

        return (null, null, input);
    }
    
    public static (string? From, string Label, string? To) ParseLocationsAndLabel(string locationPartition)
    {
        var locationsAndLabels = locationPartition.TrimEnd().Split(" ").ToList();
        
        if (locationsAndLabels.Count == 1)
        {
            return (null, locationsAndLabels.First(), null);
        }

        return (locationsAndLabels.ElementAt(0), locationsAndLabels.ElementAt(1), locationsAndLabels.ElementAt(2));
    }
    
    public static readonly Parser<List<Switch>> Switches =
        from switches in Switch.Many()
        select switches.Where(x => x is not null).ToList();
    
    public static readonly Parser<GlobalVariables> GlobalVariables =
        from identifier in Parse.String("globalVariables").Until(Parse.Char(':'))
        from values in
            Array
                .Select(x =>
                    x.Split(",").SkipWhile(string.IsNullOrEmpty)
                        .Select(s => s.Trim())
                        .Select(s =>
                            {
                                var split = s.Split(":", 2);
                                return new Variable(split[0].Trim(), split[1].Trim());
                            })
                        .ToList())
        select new GlobalVariables(values);

    public static readonly Parser<string> InitialLocation =
        from identifier in Parse.String("initialLocation").Until(Parse.Char(':'))
        from value in QuotedText
        select value;

    // TODO: Optional does not work very well within this parser. InitialLocation is not parsed properly.
    public static readonly Parser<ParsedSpecification> Spec =
        from globalVariables in GlobalVariables
        from initialLocation in InitialLocation.Optional()
        from switches in Switches
        select new ParsedSpecification(globalVariables, initialLocation.GetOrDefault(), switches);
    
    public static Specification ParseSpecification(string input)
    {
        var parsedSpecification = Spec.Parse(input);

        var parseError = ValidateSpecification(parsedSpecification);

        if (!string.IsNullOrEmpty(parseError)) throw new ParseException(parseError);
            
        parsedSpecification = CompleteDefaultLocations.PerformCompletion(parsedSpecification);
        parsedSpecification = CompleteAsynchronousOutputActions.PerformCompletion(parsedSpecification);

        var initialVariables = parsedSpecification.GlobalVariables.Variables.Select(x => x.Map()).ToList();
        
        var switches = parsedSpecification.Switches.Select(Subparts.Switch.CreateFrom).ToList();

        var initialLocation = parsedSpecification.InitialLocation ?? string.Empty;
        
        if (string.IsNullOrEmpty(initialLocation))
        {
            initialLocation = switches.First().From.Name;
        }
        
        return new Specification
        {
            InitialLocation = new Location(initialLocation),
            InitialVariables = initialVariables.ToImmutableList(),
            Switches = switches,
            CurrentState = new SpecificationState
            {
                InstantiatedLocations = new List<Location> { new(initialLocation)},
                InteractionVariables = new List<Core.Solver.Variable>(),
                LocationVariables = initialVariables
            }
        };
    }

    private static string ValidateSpecification(ParsedSpecification? parsedSpecification)
    {
        if (parsedSpecification is null) return "Input file is not parseable to a specification.";
        if (parsedSpecification.Switches
            .Where(s => s.Label.StartsWith("?"))
            .Any(s => s.IsAsync))
        {
            return "Specification not correct. Async input actions are not allowed.";
        }

        return string.Empty;
    }

}