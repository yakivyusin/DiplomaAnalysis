using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DiplomaAnalysis.Common.Extensions;

public static class WordExtensions
{
    public static IEnumerable<string> AllParagraphs(this WordprocessingDocument document)
    {
        return document
            .MainDocumentPart
            .Document
            .Descendants<Paragraph>()
            .Select(x => GetParagraphText(x))
            .Where(x => x.Length > 0);
    }

    private static string GetParagraphText(Paragraph paragraph)
    {
        return paragraph
            .Descendants<Text>()
            .Aggregate(new StringBuilder(), (builder, text) => builder.Append(text.Text))
            .ToString();
    }

    public static T ValueSafe<T>(this OpenXmlSimpleValue<T> value) where T: struct => value switch
    {
        { HasValue: true } => value.Value,
        { HasValue: false, InnerText: not null } => double.TryParse(value.InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var dValue) switch
        {
            true => (T)Convert.ChangeType(dValue, typeof(T)),
            _ => throw new ArgumentOutOfRangeException()
        },
        _ => throw new ArgumentOutOfRangeException()
    };
}
