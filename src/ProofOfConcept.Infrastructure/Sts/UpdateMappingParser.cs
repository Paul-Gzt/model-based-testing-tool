using ProofOfConcept.Core.Specifications;

namespace ProofOfConcept.Infrastructure.Sts;

public static class UpdateMappingParser
{
    public static UpdateMapping? ParseUpdateMapping(string? updateMappingAsString)
    {
        if (string.IsNullOrEmpty(updateMappingAsString)) return null;

        var updateMappingItems = updateMappingAsString.Split(" ");

        // TODO: We assume this list to have five elements

        return new UpdateMapping(
            updateMappingItems[0],
            new UpdateStatement(
                updateMappingItems[2],
                OperatorParser.ParseOperator(updateMappingItems[3]),
                updateMappingItems[4]
            )
        );
    }
}