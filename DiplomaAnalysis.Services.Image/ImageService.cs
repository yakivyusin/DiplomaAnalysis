using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiplomaAnalysis.Services.Image;

public class ImageService : IAnalysisService
{
    private readonly WordprocessingDocument _document;

    public ImageService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();
        var images = _document.MainDocumentPart
            .Document
            .Body
            .Descendants<Drawing>();

        foreach (var image in images)
        {
            var containingParagraph = image.Ancestors<Paragraph>().FirstOrDefault();
            var captionCandidateParagraph = containingParagraph.TakeNextSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

            result.AddRange(AnalyzeFormatting(image, containingParagraph, captionCandidateParagraph));
        }

        return result;
    }

    private IEnumerable<MessageDto> AnalyzeFormatting(Drawing image, OpenXmlElement containingParagraph, OpenXmlElement followingParagraph)
    {
        var followingText = followingParagraph?.InnerText ?? string.Empty;
        followingText = followingText[0..Math.Min(50, followingText.Length)];

        if (!HasImageCorrectMaxWidth(image, containingParagraph))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageWidth,
                IsError = true,
                ExtraMessage = followingText
            };
        }

        var isImageFormattingCorrect = HasParagraphCorrectFormatting(containingParagraph);

        if (!isImageFormattingCorrect)
        {
            yield return new()
            {
                Code = AnalysisCode.ImageOrCaptionFormatting,
                IsError = true,
                ExtraMessage = followingText
            };
        }

        if (followingParagraph != null && isImageFormattingCorrect && !HasParagraphCorrectFormatting(followingParagraph))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageOrCaptionFormatting,
                IsError = true,
                ExtraMessage = followingText
            };
        }
    }

    private bool HasImageCorrectMaxWidth(Drawing image, OpenXmlElement containingParagraph)
    {
        var sectionPropertiesContainer = containingParagraph.TakeNextSiblingWhile(x => x is not SectionProperties && !x.Descendants<SectionProperties>().Any());
        var pageSize = sectionPropertiesContainer.Descendants<PageSize>().FirstOrDefault();
        var pageMargin = sectionPropertiesContainer.Descendants<PageMargin>().FirstOrDefault();
        var imageSize = image.Descendants<Extent>().FirstOrDefault();

        return (imageSize.Cx / 635) - pageSize.Width + pageMargin.Left + pageMargin.Right <= 10;
    }

    private bool HasParagraphCorrectFormatting(OpenXmlElement paragraph)
    {
        static StringValue GetParagraphStyleId(OpenXmlElement element) => element.Descendants<ParagraphStyleId>().FirstOrDefault()?.Val;

        var indentation = paragraph.GetSettingFromPropertiesOrStyle<Indentation>(GetParagraphStyleId) ?? new Indentation();
        var justification = paragraph.GetSettingFromPropertiesOrStyle<Justification>(GetParagraphStyleId) ?? new Justification();

        return justification.Val == JustificationValues.Center &&
            string.IsNullOrEmpty(indentation.FirstLine) &&
            string.IsNullOrEmpty(indentation.Left) &&
            string.IsNullOrEmpty(indentation.Right) &&
            string.IsNullOrEmpty(indentation.Hanging);
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
