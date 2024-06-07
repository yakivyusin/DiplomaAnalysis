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
            var containingParagraph = image.TakeParentWhile(x => x is not Paragraph);
            var captionCandidateParagraph = containingParagraph.TakeNextSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

            if (!HasImageCorrectMaxWidth(image, containingParagraph))
            {
                result.Add(new()
                {
                    Code = AnalysisCode.ImageWidth,
                    IsError = true,
                    ExtraMessage = captionCandidateParagraph?.InnerText ?? string.Empty
                });
            }
        }

        return result;
    }

    private bool HasImageCorrectMaxWidth(Drawing image, OpenXmlElement containingParagraph)
    {
        var sectionPropertiesContainer = containingParagraph.TakeNextSiblingWhile(x => x is not SectionProperties && !x.Descendants<SectionProperties>().Any());
        var pageSize = sectionPropertiesContainer.Descendants<PageSize>().FirstOrDefault();
        var pageMargin = sectionPropertiesContainer.Descendants<PageMargin>().FirstOrDefault();
        var imageSize = image.Descendants<Extent>().FirstOrDefault();

        return (imageSize.Cx / 635) - pageSize.Width + pageMargin.Left + pageMargin.Right <= 10;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
