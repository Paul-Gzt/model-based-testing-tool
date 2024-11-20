using System.Globalization;
using System.Text;

namespace ProofOfConcept.Infrastructure.Utility;

public static class DapperUtility
{
    public static string DapperQueryToCsvString(IEnumerable<dynamic> dapperQueryResults)
    {
        var queryResults = dapperQueryResults.ToList();
        if (!queryResults.Any()) return string.Empty;

        var stringWriter = new StringWriter();

        WriteDapperQueryToText(queryResults, stringWriter);
        return stringWriter.ToString();
    }

    public static Stream DapperQueryToCsvStream(IEnumerable<dynamic> dapperQueryResults)
    {
        var queryResults = dapperQueryResults.ToList();
        if (!queryResults.Any()) return new MemoryStream();
        
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);

        WriteDapperQueryToText(queryResults, streamWriter);
        streamWriter.Flush();
        return memoryStream;
    }

    private static void WriteDapperQueryToText(IEnumerable<dynamic> dapperQueryResults, TextWriter tw)
    {
        var csv = new CsvHelper.CsvWriter(tw, CultureInfo.InvariantCulture);
        var dapperRows = dapperQueryResults.Cast<IDictionary<string, object>>().ToList();
        var headerWritten = false;
        
        foreach (IDictionary<string, object> row in dapperRows)
        {
            if (!headerWritten)
            {
                foreach (KeyValuePair<string, object> item in row)
                {
                    csv.WriteField(item.Key);
                }
                csv.NextRecord();
                headerWritten = true;
            }
            foreach (KeyValuePair<string, object> item in row)
            {
                csv.WriteField(item.Value);
            }
            csv.NextRecord();
        }
        csv.Flush();
    }
}