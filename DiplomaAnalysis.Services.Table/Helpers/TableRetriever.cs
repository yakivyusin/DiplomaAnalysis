using DiplomaAnalysis.Common.Extensions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace DiplomaAnalysis.Services.Table.Helpers;

internal class TableRetriever
{
    public IEnumerable<WordTable> RetrieveTables(WordprocessingDocument document)
    {
        return document.MainDocumentPart
            .Document
            .Body
            .OfType<WordTable>()
            .Where(IsTableSuitableForAnalysis);
    }

    private static bool IsTableSuitableForAnalysis(WordTable table)
    {
        static bool IsVisibleBorder(EnumValue<BorderValues> value) => value != BorderValues.None && value != BorderValues.Nil;

        var tableBorders = table.GetSettingFromPropertiesOrStyle<TableBorders>(x => x.Descendants<TableStyle>().FirstOrDefault()?.Val);

        return tableBorders != null &&
            IsVisibleBorder(tableBorders.TopBorder?.Val) &&
            IsVisibleBorder(tableBorders.BottomBorder?.Val) &&
            IsVisibleBorder(tableBorders.LeftBorder?.Val) &&
            IsVisibleBorder(tableBorders.RightBorder?.Val);
    }
}
