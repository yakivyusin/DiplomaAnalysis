using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;
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
        static StringValue GetTableStyleId(OpenXmlElement element) => element.Descendants<TableStyle>().FirstOrDefault()?.Val;

        var tableLayout = table.GetSettingFromPropertiesOrStyle<TableLayout>(GetTableStyleId);
        var tableWidth = table.GetSettingFromPropertiesOrStyle<TableWidth>(GetTableStyleId);

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

            return Math.Abs(pageSize.Width - pageMargin.Left - pageMargin.Right - decimal.Parse(tableWidth.Width, CultureInfo.InvariantCulture)) < 20;
        }

        if (tableWidth?.Type == TableWidthUnitValues.Pct)
        {
            return Math.Abs(decimal.Parse(tableWidth.Width, CultureInfo.InvariantCulture) - 5000) < 20;
        }

        return true;
    }
}
