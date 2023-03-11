using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.CharReplacement
{
    public class CharReplacementService : IDisposable
    {
        private static readonly Regex[] _regexes = new Regex[]
        {
            new(@"\p{IsCyrillic}[A-Za-z]+[\p{IsCyrillic}]", RegexOptions.Compiled),
            new(@"[A-Za-z][\p{IsCyrillic}]+[A-Za-z]", RegexOptions.Compiled)
        };
        private readonly WordprocessingDocument _document;

        public CharReplacementService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            return _regexes
                .SelectMany(x => Analyze(_document.AllParagraphs(), x));
        }

        private IEnumerable<MessageDto> Analyze(IEnumerable<string> texts, Regex regex)
        {
            return texts
                .SelectMany(x => regex.Matches(x).Select(m => new { Match = m, Text = x }))
                .Select(x => new MessageDto
                {
                    Code = AnalysisCode.CharacterReplacement,
                    IsError = true,
                    ExtraMessage = x.Match.GetMatchTextWithContext(x.Text, 15)
                });
        }

        public void Dispose()
        {
            ((IDisposable)_document).Dispose();
        }
    }
}
