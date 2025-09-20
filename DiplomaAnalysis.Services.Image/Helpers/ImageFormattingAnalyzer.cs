using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaAnalysis.Services.Image.Helpers;

internal class ImageFormattingAnalyzer
{
    public IEnumerable<MessageDto> Analyze(Drawing image)
    {
        var containingParagraph = image.Ancestors<Paragraph>().FirstOrDefault();
        var followingParagraph = containingParagraph.TakeNextSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

        if (!HasImageCorrectMaxWidth(image, containingParagraph, out var pageWidth, out var imageWidth))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageWidth,
                IsError = true,
                ExtraMessage = followingParagraph?.InnerText.TakeFirst(50)
            };
        }

        if (!HasParagraphCorrectFormatting(containingParagraph, pageWidth - imageWidth > 20) ||
            (followingParagraph != null && !HasParagraphCorrectFormatting(followingParagraph, true)))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageOrCaptionFormatting,
                IsError = true,
                ExtraMessage = followingParagraph?.InnerText.TakeFirst(50)
            };
        }
    }

    private static bool HasImageCorrectMaxWidth(Drawing image, OpenXmlElement containingParagraph, out uint pageWidth, out long imageWidth)
    {
        var sectionPropertiesContainer = containingParagraph.TakeNextSiblingWhile(x => x is not SectionProperties && !x.Descendants<SectionProperties>().Any());
        var pageSize = sectionPropertiesContainer.Descendants<PageSize>().FirstOrDefault();
        var pageMargin = sectionPropertiesContainer.Descendants<PageMargin>().FirstOrDefault();
        var imageSize = image.Descendants<Extent>().FirstOrDefault();

        pageWidth = pageSize.Width - pageMargin.Left - pageMargin.Right;
        imageWidth = imageSize.Cx / 635;

        return imageWidth - pageWidth <= 10;
    }

    private static bool HasParagraphCorrectFormatting(OpenXmlElement paragraph, bool requireCenterJustification)
    {
        static StringValue GetParagraphStyleId(OpenXmlElement element) => element.Descendants<ParagraphStyleId>().FirstOrDefault()?.Val;

        var indentation = paragraph.GetSettingFromPropertiesOrStyle<Indentation>(GetParagraphStyleId) ?? new Indentation();
        var justification = paragraph.GetSettingFromPropertiesOrStyle<Justification>(GetParagraphStyleId) ?? new Justification { Val = JustificationValues.Start };

        return (!requireCenterJustification || justification.Val == JustificationValues.Center) &&
            (string.IsNullOrEmpty(indentation.FirstLine) || int.Parse(indentation.FirstLine) <= 10) &&
            (string.IsNullOrEmpty(indentation.Left) || int.Parse(indentation.Left) <= 10) &&
            (string.IsNullOrEmpty(indentation.Right) || int.Parse(indentation.Right) <= 10) &&
            (string.IsNullOrEmpty(indentation.Hanging) || int.Parse(indentation.Hanging) <= 10);
    }
}
