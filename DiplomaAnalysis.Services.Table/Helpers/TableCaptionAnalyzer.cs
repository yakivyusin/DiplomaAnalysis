using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace DiplomaAnalysis.Services.Table.Helpers;

internal class TableCaptionAnalyzer
{
    private const string TableReferencePattern = @"(?i)табл(\.|иц[ію])[ \xa0]{0}\.{1}";
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

        Func<OpenXmlElement, Match, IEnumerable<MessageDto>> followingAnalysis = captionMatch.Value.StartsWith('Т') ?
            AnalyzeTableStart :
            AnalyzeTableContinuation;

        foreach (var error in followingAnalysis(sibling, captionMatch))
        {
            yield return error;
        }
    }

    private static IEnumerable<MessageDto> AnalyzeTableStart(OpenXmlElement tablePreviousSibling, Match captionMatch)
    {
        var (chapter, order) = RetrieveCaptionInfo(captionMatch);
        var captionStartElement = tablePreviousSibling.PreviousSibling();

        if (!HasTableReference(captionStartElement, chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.TableReference,
                IsError = true,
                ExtraMessage = captionMatch.Value.TakeFirst(50)
            };
        }

        if (!HasTablePredecessor(captionStartElement, chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.TableNumbering,
                IsError = true,
                ExtraMessage = captionMatch.Value.TakeFirst(50)
            };
        }
    }

    private static IEnumerable<MessageDto> AnalyzeTableContinuation(OpenXmlElement tablePreviousSibling, Match captionMatch)
    {
        var (chapter, order) = RetrieveCaptionInfo(captionMatch);
        var captionStartElement = tablePreviousSibling;

        if (!HasTableStartOnPreviousPage(captionStartElement, chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.TableContinuation,
                IsError = true,
                ExtraMessage = captionStartElement.InnerText.TakeFirst(50)
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

    private static bool HasTableStartOnPreviousPage(OpenXmlElement captionStartElement, int chapter, int order)
    {
        if (captionStartElement.Descendants<LastRenderedPageBreak>().Any())
        {
            return true;
        }

        var tableStartRegex = new Regex(string.Format(TableStartPattern, chapter, order));
        var firstMatch = captionStartElement.TakePreviousSiblingUntil(x =>
            tableStartRegex.IsMatch(x.InnerText) ||
            x.Descendants<Break>().Any() ||
            x.Descendants<LastRenderedPageBreak>().Any());

        return firstMatch.Descendants<Break>().Any() || firstMatch.Descendants<LastRenderedPageBreak>().Any();
    }
}
