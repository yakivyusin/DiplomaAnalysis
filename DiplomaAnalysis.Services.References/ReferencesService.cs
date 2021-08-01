using DiplomaAnalysis.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.References
{
    public class ReferencesService : IDisposable
    {
        private const string ReferencePattern = @"\[\d+\]";
        private readonly WordprocessingDocument _document;

        public ReferencesService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            var regex = new Regex(ReferencePattern);
            var isAnyTextContainsReference = _document
                .MainDocumentPart
                .Document
                .Descendants<Text>()
                .Any(x => regex.IsMatch(x.Text));

            if (isAnyTextContainsReference)
            {
                return Enumerable.Empty<MessageDto>();
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
