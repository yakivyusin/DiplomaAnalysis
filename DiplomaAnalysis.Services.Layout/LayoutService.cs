using DiplomaAnalysis.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiplomaAnalysis.Services.Layout
{
    public class LayoutService : IDisposable
    {
        private static readonly int ShortSize = (int)Math.Round(21.0 * 1440 / 2.54);
        private static readonly int LongSize = (int)Math.Round(29.7 * 1440 / 2.54);
        private static readonly int LeftMargin = (int)Math.Round(3.0 * 1440 / 2.54);
        private static readonly int OtherMargins = (int)Math.Round(2.0 * 1440 / 2.54);
        private static readonly int AllowedDifference = 10;

        private readonly WordprocessingDocument _document;

        public LayoutService(Stream data)
        {
            _document = WordprocessingDocument.Open(data, false);
        }

        public IEnumerable<MessageDto> Analyze()
        {
            var sectionProperties = _document
                .MainDocumentPart
                .Document
                .Body
                .Descendants<SectionProperties>()
                .ToList();

            if (sectionProperties
                .Select(x => x.GetFirstChild<PageSize>())
                .Any(x => !IsPageSizeCorrect(x)))
            {
                yield return new MessageDto
                {
                    Code = AnalysisCode.PageSize,
                    IsError = true,
                    ExtraMessage = null
                };
            }

            if (sectionProperties
                .Select(x => x.GetFirstChild<PageMargin>())
                .Any(x => !IsPageMarginCorrect(x)))
            {
                yield return new MessageDto
                {
                    Code = AnalysisCode.PageMargin,
                    IsError = false,
                    ExtraMessage = null
                };
            }
        }

        private bool IsPageSizeCorrect(PageSize size)
        {
            if (size.Orient == null || size.Orient.Value == PageOrientationValues.Portrait)
            {
                return
                    Math.Abs(size.Width - ShortSize) <= AllowedDifference &&
                    Math.Abs(size.Height - LongSize) <= AllowedDifference;
            }
            else
            {
                return
                    Math.Abs(size.Width - LongSize) <= AllowedDifference &&
                    Math.Abs(size.Height - ShortSize) <= AllowedDifference;
            }
        }

        private bool IsPageMarginCorrect(PageMargin margin)
        {
            return
                Math.Abs(margin.Left - LeftMargin) <= AllowedDifference &&
                Math.Abs(margin.Right - OtherMargins) <= AllowedDifference &&
                Math.Abs(margin.Top - OtherMargins) <= AllowedDifference &&
                Math.Abs(margin.Bottom - OtherMargins) <= AllowedDifference;
        }

        public void Dispose()
        {
            ((IDisposable)_document).Dispose();
        }
    }
}
