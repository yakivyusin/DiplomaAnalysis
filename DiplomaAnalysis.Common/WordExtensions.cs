using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaAnalysis.Common
{
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
    }
}
