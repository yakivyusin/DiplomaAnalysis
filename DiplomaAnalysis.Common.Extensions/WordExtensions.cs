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
            .Select(GetParagraphText)
            .Where(x => x.Length > 0);
    }

    private static string GetParagraphText(Paragraph paragraph)
    {
        return paragraph
            .Descendants<Run>()
            .Where(x => !IsSourceCode(paragraph, x))
            .SelectMany(x => x.Descendants<Text>())
            .Aggregate(new StringBuilder(), (builder, text) => builder.Append(text.Text))
            .ToString();
    }

    private static bool IsSourceCode(Paragraph paragraph, Run run)
    {
        var runFonts = run.GetSettingFromPropertiesOrStyle<RunFonts>((_) => paragraph.Descendants<ParagraphStyleId>().FirstOrDefault()?.Val);

        return runFonts != null &&
            (runFonts.Ascii == "Courier New" || runFonts.Ascii == "Consolas" ||
             runFonts.HighAnsi == "Courier New" || runFonts.HighAnsi == "Consolas");
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

    public static T GetSettingFromPropertiesOrStyle<T>(this OpenXmlElement element, Func<OpenXmlElement, StringValue> styleIdSelector) where T: OpenXmlElement
    {
        var settingElement = element.Descendants<T>().FirstOrDefault();

        if (settingElement != null)
        {
            return settingElement;
        }

        var styleId = styleIdSelector(element);
        if (styleId != null)
        {
            var document = element.Ancestors<Document>().First();
            var styles = document.MainDocumentPart.StyleDefinitionsPart.Styles.OfType<Style>();

            while (styleId != null && settingElement == null)
            {
                var style = styles.FirstOrDefault(x => x.StyleId == styleId);

                settingElement = style?.Descendants<T>().FirstOrDefault();
                styleId = style?.BasedOn?.Val;
            }
        }
        else
        {
            var document = element.Ancestors<Document>().First();

            settingElement = document
                .MainDocumentPart
                .StyleDefinitionsPart
                .Styles
                .OfType<Style>()
                .FirstOrDefault(x => x.StyleName.Val == "Normal")
                ?.Descendants<T>()
                .FirstOrDefault();
        }

        return settingElement;
    }

    public static OpenXmlElement TakePreviousSiblingWhile(this OpenXmlElement element, Func<OpenXmlElement, bool> predicate) =>
        element.TakeElementFromCurrentWhile(x => x.PreviousSibling(), predicate);

    public static OpenXmlElement TakeNextSiblingWhile(this OpenXmlElement element, Func<OpenXmlElement, bool> predicate) =>
        element.TakeElementFromCurrentWhile(x => x.NextSibling(), predicate);

    public static OpenXmlElement TakeParentWhile(this OpenXmlElement element, Func<OpenXmlElement, bool> predicate) =>
        element.TakeElementFromCurrentWhile(x => x.Parent, predicate);

    private static OpenXmlElement TakeElementFromCurrentWhile(this OpenXmlElement element, Func<OpenXmlElement, OpenXmlElement> nextFunc, Func<OpenXmlElement, bool> predicate)
    {
        if (element == null)
        {
            return null;
        }

        var next = nextFunc(element);

        while (next != null && predicate(next))
        {
            next = nextFunc(next);
        }

        return next;
    }
}
