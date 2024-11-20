using Newtonsoft.Json.Linq;

namespace ProofOfConcept.Infrastructure.Templates;

public static class JTokenExtensions
{
    public static JToken RemoveFields(this JToken token, IEnumerable<string> fieldsQuery)
    {
        var fieldsList = fieldsQuery.ToList();
        if (token is not JContainer container) return token;

        var removeList = new List<JToken>();
        foreach (var element in container.Children())
        {
            if (element is JProperty property && fieldsList.Contains(property.Name))
            {
                removeList.Add(element);
            }
            element.RemoveFields(fieldsList);
        }

        foreach (var element in removeList)
        {
            element.Remove();
        }

        return token;
    }
    
    public static (JToken, List<string>) RemoveValues(this JToken token, string[] values)
    {
        if (token is not JContainer container) return (token, new List<string>());
        
        var removedFields = new List<string>();
        var removeList = new List<JToken>();
        foreach (var element in container.Children())
        {
            if (element is JProperty property && values.Contains(property.Value.ToString()))
            {
                removeList.Add(element);
                removedFields.Add(((JProperty)element).Name);
            }
            var (_, removedFieldsFromChildren) = element.RemoveValues(values);
            removedFields.AddRange(removedFieldsFromChildren);
        }

        foreach (var element in removeList)
        {
            element.Remove();
        }

        return (token, removedFields);
    }
}