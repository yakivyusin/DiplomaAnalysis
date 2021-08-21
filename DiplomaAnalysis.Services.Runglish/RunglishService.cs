using DiplomaAnalysis.Common;
using DiplomaAnalysis.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Runglish
{
    public class RunglishService : IDisposable
    {
        private readonly WordprocessingDocument _document;

        public RunglishService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            return Properties
                .Resources
                .Terms
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(x => Analyze(_document.AllParagraphs(), x));
        }

        private IEnumerable<MessageDto> Analyze(IEnumerable<string> texts, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return texts
                .SelectMany(x => regex.Matches(x).Select(m => new { Match = m, Text = x }))
                .Select(x => new MessageDto
                {
                    Code = AnalysisCode.Runglish,
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
