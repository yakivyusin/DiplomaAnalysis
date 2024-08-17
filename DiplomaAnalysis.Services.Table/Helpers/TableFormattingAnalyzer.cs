using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace DiplomaAnalysis.Services.Table.Helpers;

internal class TableFormattingAnalyzer
{
    public IEnumerable<MessageDto> Analyze(WordTable table)
    {
        if (IsTableFullWidth(table))
        {
            yield break;
        }

        var sibling = table.TakePreviousSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

        yield return new()
        {
            Code = AnalysisCode.TableWidth,
            IsError = true,
            ExtraMessage = sibling?.InnerText.TakeLast(50)
        };
    }

    private static bool IsTableFullWidth(WordTable table)
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
}
