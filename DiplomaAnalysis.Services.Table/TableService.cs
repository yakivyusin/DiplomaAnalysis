﻿using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace DiplomaAnalysis.Services.Table;

public class TableService : IAnalysisService
{
    private const string TableReferencePattern = @"(?i)табл(\.|иці)[ \xa0]{0}.{1}";
    private static readonly Regex _captionRegex = new(@"(?is)(^Таблиця\s(?<chapter>\d+)\.(?<order>\d+).+\S+$|Продовження\sтабл\.\s(?<chapter>\d+)\.(?<order>\d+)$)", RegexOptions.Compiled);
    private readonly WordprocessingDocument _document;

    public TableService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();
        var numbering = new List<(int, int)>();
        var documentTables = _document.MainDocumentPart
            .Document
            .Body
            .OfType<WordTable>();

        foreach (var table in documentTables.Where(x => IsTableSuitableForAnalysis(x)))
        {
            var sibling = table.TakePreviousSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));
            var siblingsText = new StringBuilder(sibling?.InnerText ?? string.Empty)
                .Insert(0, Environment.NewLine)
                .Insert(0, sibling?.PreviousSibling()?.InnerText ?? string.Empty)
                .ToString();
            var shortSiblingsText = siblingsText[Math.Max(0, siblingsText.Length - 50)..];

            if (!IsTableFullWidth(table))
            {
                result.Add(new()
                {
                    Code = AnalysisCode.TableWidth,
                    IsError = true,
                    ExtraMessage = shortSiblingsText
                });
            }

            var captionMatch = _captionRegex.Match(siblingsText);

            if (captionMatch == Match.Empty)
            {
                result.Add(new()
                {
                    Code = AnalysisCode.TableCaption,
                    IsError = true,
                    ExtraMessage = shortSiblingsText
                });

                continue;
            }

            result.AddRange(AnalyzeTableWithStartingCaption(captionMatch, sibling.PreviousSibling(), numbering));
        }

        return result;
    }

    private bool IsTableFullWidth(WordTable table)
    {
        var tableLayout = table.Descendants<TableLayout>().FirstOrDefault();
        var tableWidth = table.Descendants<TableWidth>().FirstOrDefault();

        if ((tableLayout?.Type ?? TableLayoutValues.Autofit) == TableLayoutValues.Autofit &&
            (tableWidth?.Type ?? TableWidthUnitValues.Auto) == TableWidthUnitValues.Auto)
        {
            return true;
        }

        if (tableWidth?.Type == TableWidthUnitValues.Dxa)
        {
            var sectionPropertiesContainer = table.TakeNextSiblingWhile(x => x is not SectionProperties && !x.Descendants<SectionProperties>().Any());
            var pageSize = sectionPropertiesContainer.Descendants<PageSize>().FirstOrDefault();
            var pageMargin = sectionPropertiesContainer.Descendants<PageMargin>().FirstOrDefault();

            return Math.Abs(pageSize.Width - pageMargin.Left - pageMargin.Right - int.Parse(tableWidth.Width)) < 20;
        }

        if (tableWidth?.Type == TableWidthUnitValues.Pct)
        {
            return Math.Abs(int.Parse(tableWidth.Width) - 5000) < 20;
        }

        return true;
    }

    private IEnumerable<MessageDto> AnalyzeTableWithStartingCaption(Match captionMatch, OpenXmlElement captionStartElement, List<(int chapter, int order)> previousTables)
    {
        if (!captionMatch.Value.StartsWith("Т", StringComparison.InvariantCultureIgnoreCase))
        {
            yield break;
        }

        var chapterNumber = int.Parse(captionMatch.Groups["chapter"].Value);
        var orderNumber = int.Parse(captionMatch.Groups["order"].Value);

        var referenceRegex = new Regex(string.Format(TableReferencePattern, chapterNumber, orderNumber));
        var referenceParagraph = captionStartElement.TakePreviousSiblingWhile(x => !referenceRegex.IsMatch(x.InnerText));

        if (referenceParagraph == null)
        {
            yield return new()
            {
                Code = AnalysisCode.TableReference,
                IsError = true,
                ExtraMessage = captionMatch.Value
            };
        }

        var previousChapterTables = previousTables.Where(x => x.chapter == chapterNumber);

        if ((orderNumber != 1 && !previousChapterTables.Any(x => x.order == orderNumber - 1)) ||
            previousChapterTables.Any(x => x.order == orderNumber))
        {
            yield return new()
            {
                Code = AnalysisCode.TableNumbering,
                IsError = true,
                ExtraMessage = captionMatch.Value
            };
        }

        previousTables.Add((chapterNumber, orderNumber));
    }

    private bool IsTableSuitableForAnalysis(WordTable table)
    {
        static bool IsVisibleBorder(EnumValue<BorderValues> value) => value != BorderValues.None && value != BorderValues.Nil;

        var tableBorders = table.GetSettingFromPropertiesOrStyle<TableBorders>(x => x.Descendants<TableStyle>().FirstOrDefault()?.Val);

        return tableBorders != null &&
            IsVisibleBorder(tableBorders.TopBorder?.Val) &&
            IsVisibleBorder(tableBorders.BottomBorder?.Val) &&
            IsVisibleBorder(tableBorders.LeftBorder?.Val) &&
            IsVisibleBorder(tableBorders.RightBorder?.Val);
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
