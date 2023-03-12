using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.References
{
    public sealed class ReferencesService : IAnalysisService
    {
        private const string DstuReferencePattern = @"\[\d+\]";
        private const string ApaReferencePattern = @"\(\p{L}.+, \d{4}[a-z]?\)";
        private readonly WordprocessingDocument _document;

        public ReferencesService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IReadOnlyCollection<MessageDto> Analyze()
        {
            var dstuRegex = new Regex(DstuReferencePattern, RegexOptions.Compiled);
            var apaPattern = new Regex(ApaReferencePattern, RegexOptions.Compiled);

            var isAnyTextContainsReference = _document
                .AllParagraphs()
                .Any(x => dstuRegex.IsMatch(x) || apaPattern.IsMatch(x));

            if (isAnyTextContainsReference)
            {
                return Enumerable
                    .Empty<MessageDto>()
                    .ToList();
            }
            else
            {
                return new[]
                {
                    new MessageDto
                    {
                        Code = AnalysisCode.References,
                        IsError = false,
                        ExtraMessage = null
                    }
                };
            }
        }

        public void Dispose()
        {
            ((IDisposable)_document).Dispose();
        }
    }
}
