using DiplomaAnalysis.Common;
using DiplomaAnalysis.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Punctuation
{
    public class PunctuationService : IDisposable
    {
        private static readonly Regex[] _punctuationSpacingRegexes = new Regex[]
        {
            new(@"\p{IsCyrillic}[.,:;?!]\p{IsCyrillic}", RegexOptions.Compiled),
            new(@"\p{IsCyrillic}\s[.,:;?!]\s\p{IsCyrillic}", RegexOptions.Compiled),
            new(@"\p{IsCyrillic}\s[.,:;?!]\p{IsCyrillic}", RegexOptions.Compiled)
        };
        private static readonly Regex _quotesRegex = new(@"[“”„""]", RegexOptions.Compiled);
        private readonly WordprocessingDocument _document;

        public PunctuationService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            return _punctuationSpacingRegexes
                .Select(x => (regex: x, code: AnalysisCode.PunctuationSpacing, isError: true))
                .Append((_quotesRegex, AnalysisCode.Quotes, isError: false))
                .SelectMany(x => Analyze(_document.AllParagraphs(), x));
        }

        private IEnumerable<MessageDto> Analyze(IEnumerable<string> texts, (Regex regex, string code, bool isError) rule)
        {
            return texts
                .SelectMany(x => rule.regex.Matches(x).Select(m => new { Match = m, Text = x }))
                .Select(x => new MessageDto
                {
                    Code = rule.code,
                    IsError = rule.isError,
                    ExtraMessage = x.Match.GetMatchTextWithContext(x.Text, 10)
                });
        }

        public void Dispose()
        {
            ((IDisposable)_document).Dispose();
        }
    }
}
