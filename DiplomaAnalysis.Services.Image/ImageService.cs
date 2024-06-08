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
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Image;

public class ImageService : IAnalysisService
{
    private const string ImageReferencePattern = @"(?i)рис(\.|унок|унку)[ \xa0]{0}.{1}";
    private static readonly Regex _captionRegex = new(@"^Рис\.\s(?<chapter>\d+)\.(?<order>\d+)\.\s\S+", RegexOptions.Compiled);
    private readonly WordprocessingDocument _document;

    public ImageService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();
        var numbering = new List<(int, int)>();
        var images = _document.MainDocumentPart
            .Document
            .Body
            .Descendants<Drawing>();

        foreach (var image in images)
        {
            var containingParagraph = image.Ancestors<Paragraph>().FirstOrDefault();
            var captionCandidateParagraph = containingParagraph.TakeNextSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

            result.AddRange(AnalyzeCaption(containingParagraph, captionCandidateParagraph, numbering));
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

    private IEnumerable<MessageDto> AnalyzeCaption(OpenXmlElement containingParagraph, OpenXmlElement followingParagraph, List<(int chapter, int order)> numbering)
    {
        var followingText = followingParagraph?.InnerText ?? string.Empty;
        followingText = followingText[0..Math.Min(50, followingText.Length)];

        var captionMatch = _captionRegex.Match(followingParagraph?.InnerText ?? string.Empty);

        if (captionMatch == Match.Empty)
        {
            yield return new()
            {
                Code = AnalysisCode.ImageCaption,
                IsError = true,
                ExtraMessage = followingText
            };

            yield break;
        }

        var chapterNumber = int.Parse(captionMatch.Groups["chapter"].Value);
        var orderNumber = int.Parse(captionMatch.Groups["order"].Value);

        var referenceRegex = new Regex(string.Format(ImageReferencePattern, chapterNumber, orderNumber));
        var referenceParagraph = containingParagraph.TakePreviousSiblingWhile(x => !referenceRegex.IsMatch(x.InnerText));

        if (referenceParagraph == null)
        {
            yield return new()
            {
                Code = AnalysisCode.ImageReference,
                IsError = true,
                ExtraMessage = followingText
            };
        }

        var chapterNumbering = numbering.Where(x => x.chapter == chapterNumber);

        if ((orderNumber != 1 && !chapterNumbering.Any(x => x.order == orderNumber - 1)) ||
            chapterNumbering.Any(x => x.order == orderNumber))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageNumbering,
                IsError = true,
                ExtraMessage = captionMatch.Value
            };
        }

        numbering.Add((chapterNumber, orderNumber));
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
        var justification = paragraph.GetSettingFromPropertiesOrStyle<Justification>(GetParagraphStyleId) ?? new Justification { Val = JustificationValues.Start };

        return justification.Val == JustificationValues.Center &&
            (string.IsNullOrEmpty(indentation.FirstLine) || int.Parse(indentation.FirstLine) <= 10) &&
            (string.IsNullOrEmpty(indentation.Left) || int.Parse(indentation.Left) <= 10) &&
            (string.IsNullOrEmpty(indentation.Right) || int.Parse(indentation.Right) <= 10) &&
            (string.IsNullOrEmpty(indentation.Hanging) || int.Parse(indentation.Hanging) <= 10);
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
