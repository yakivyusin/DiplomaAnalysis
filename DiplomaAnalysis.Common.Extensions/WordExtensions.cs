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
            var document = (Document)element.TakeParentWhile(x => x is not Document);
            var styles = document.MainDocumentPart.StyleDefinitionsPart.Styles.OfType<Style>();

            settingElement = styles.FirstOrDefault(x => x.StyleId == styleId)?.Descendants<T>().FirstOrDefault();
        }

        return settingElement;
    }

    public static OpenXmlElement TakePreviousSiblingWhile(this OpenXmlElement element, Func<OpenXmlElement, bool> predicate)
    {
        if (element == null)
        {
            return null;
        }

        var sibling = element.PreviousSibling();

        while (sibling != null && predicate(sibling))
        {
            sibling = sibling.PreviousSibling();
        }

        return sibling;
    }

    public static OpenXmlElement TakeParentWhile(this OpenXmlElement element, Func<OpenXmlElement, bool> predicate)
    {
        var parent = element.Parent;

        while (parent != null && predicate(parent))
        {
            parent = parent.Parent;
        }

        return parent;
    }
}
