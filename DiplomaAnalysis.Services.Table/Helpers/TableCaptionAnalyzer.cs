using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace DiplomaAnalysis.Services.Table.Helpers;

internal class TableCaptionAnalyzer
{
    private const string TableReferencePattern = @"(?i)табл(\.|иці)[ \xa0]{0}\.{1}";
    private const string TableStartPattern = @"(?i)Таблиця {0}\.{1}";
    private static readonly Regex _captionRegex = new(@"(?is)(^Таблиця\s(?<chapter>\d+)\.(?<order>\d+).+\S+$|Продовження\sтабл\.\s(?<chapter>\d+)\.(?<order>\d+)$)", RegexOptions.Compiled);

    public IEnumerable<MessageDto> Analyze(WordTable table)
    {
        var sibling = table.TakePreviousSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));
        var twoSiblingsText = new StringBuilder(sibling?.PreviousSibling()?.InnerText)
            .AppendLine()
            .Append(sibling?.InnerText)
            .ToString();

        var captionMatch = _captionRegex.Match(twoSiblingsText);

        if (!captionMatch.Success)
        {
            yield return new()
            {
                Code = AnalysisCode.TableCaption,
                IsError = true,
                ExtraMessage = twoSiblingsText.TakeLast(50)
            };

            yield break;
        }

        if (!captionMatch.Value.StartsWith("Т", StringComparison.InvariantCultureIgnoreCase))
        {
            yield break;
        }

        var (chapter, order) = RetrieveCaptionInfo(captionMatch);

        if (!HasTableReference(sibling.PreviousSibling(), chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.TableReference,
                IsError = true,
                ExtraMessage = captionMatch.Value.TakeFirst(50)
            };
        }

        if (!HasTablePredecessor(sibling.PreviousSibling(), chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.TableNumbering,
                IsError = true,
                ExtraMessage = captionMatch.Value.TakeFirst(50)
            };
        }
    }

    private static (int chapter, int order) RetrieveCaptionInfo(Match captionMatch) =>
        (int.Parse(captionMatch.Groups["chapter"].Value), int.Parse(captionMatch.Groups["order"].Value));

    private static bool HasTableReference(OpenXmlElement captionStartElement, int chapter, int order)
    {
        var referenceRegex = new Regex(string.Format(TableReferencePattern, chapter, order));

        return captionStartElement.TakePreviousSiblingWhile(x => !referenceRegex.IsMatch(x.InnerText)) != null;
    }

    private static bool HasTablePredecessor(OpenXmlElement captionStartElement, int chapter, int order)
    {
        if (order == 1)
        {
            return true;
        }

        var previousNumberRegex = new Regex(string.Format(TableStartPattern, chapter, order - 1));

        return captionStartElement.TakePreviousSiblingWhile(x => !previousNumberRegex.IsMatch(x.InnerText)) != null;
    }
}
