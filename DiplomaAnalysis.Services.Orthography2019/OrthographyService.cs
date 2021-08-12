using DiplomaAnalysis.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Orthography2019
{
    public class OrthographyService : IDisposable
    {
        private readonly WordprocessingDocument _document;

        public OrthographyService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            var texts = _document
                .MainDocumentPart
                .Document
                .Descendants<Text>()
                .ToList();

            return Properties
                .Resources
                .Terms
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(x => Analyze(texts, x));
        }

        private IEnumerable<MessageDto> Analyze(IEnumerable<Text> texts, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return texts
                .SelectMany(x => regex.Matches(x.Text).Select(m => new { Match = m, x.Text }))
                .Select(x => new MessageDto
                {
                    Code = AnalysisCode.Orthography2019,
                    IsError = true,
                    ExtraMessage = GetMatchText(x.Match, x.Text)
                });
        }

        private string GetMatchText(Match match, string text)
        {
            var startIndex = Math.Max(0, match.Index - 20);
            var length = Math.Min(text.Length - startIndex, (match.Index - startIndex) + match.Value.Length + (match.Index - startIndex));

            return text
                .Substring(startIndex, length);
        }

        public void Dispose()
        {
            ((IDisposable)_document).Dispose();
        }
    }
}
