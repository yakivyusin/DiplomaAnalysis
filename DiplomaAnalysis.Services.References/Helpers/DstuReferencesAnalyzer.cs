using DiplomaAnalysis.Common.Extensions;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.References.Helpers;

internal class DstuReferencesAnalyzer
{
    private static readonly Regex _dstuReferenceRegex = new(@"\[(\d+|\d+[-–—]\d+)(?:,\s*(\d+|\d+[-–—]\d+)+)*\]");
    private static readonly Regex _numberRangeRegex = new(@"(\d+)[-–—](\d+)");

    public (bool areReferencesExist, bool areReferencesInCorrectOrder) Analyze(WordprocessingDocument document)
    {
        var dstuReferences = document
            .AllParagraphs()
            .SelectMany(x => _dstuReferenceRegex.Matches(x))
            .ToList();

        if (dstuReferences.Count == 0)
        {
            return (false, true);
        }

        return (true, AreDstuInCorrectOrder(dstuReferences.SelectMany(ExpandReference)));
    }

    private static IEnumerable<int> ExpandReference(Match referenceMatch) => referenceMatch.Groups
        .Values
        .Skip(1)
        .SelectMany(x => x.Captures)
        .SelectMany(x =>
        {
            var numberRangeMatch = _numberRangeRegex.Match(x.Value);

            if (numberRangeMatch.Success)
            {
                var start = int.Parse(numberRangeMatch.Groups[1].Value);

                return Enumerable.Range(start, int.Parse(numberRangeMatch.Groups[2].Value) - start + 1);
            }
            else
            {
                return [int.Parse(x.Value)];
            }
        });

    private static bool AreDstuInCorrectOrder(IEnumerable<int> references)
    {
        var list = references.ToList();

        if (list[0] != 1)
        {
            return false;
        }

        foreach (var (number, index) in list.Select((x, index) => (number: x, index)).Where(x => x.number > 1))
        {
            if (!list.Take(index).Any(x => x == number - 1))
            {
                return false;
            }
        }

        return true;
    }
}
