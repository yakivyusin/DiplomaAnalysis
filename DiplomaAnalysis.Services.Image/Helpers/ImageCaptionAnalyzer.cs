using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Image.Helpers;

internal class ImageCaptionAnalyzer
{
    private const string ImageReferencePattern = @"(?i)рис(\.|унок|унку)[ \xa0]{0}\.{1}";
    private const string ImageStartPattern = @"(?i)Рис\. {0}\.{1}";
    private static readonly Regex _captionRegex = new(@"^Рис\.\s(?<chapter>\d+)\.(?<order>\d+)\.\s\S+", RegexOptions.Compiled);

    public IEnumerable<MessageDto> Analyze(Drawing image)
    {
        var containingParagraph = image.Ancestors<Paragraph>().FirstOrDefault();
        var followingParagraph = containingParagraph.TakeNextSiblingWhile(x => string.IsNullOrEmpty(x.InnerText));

        var captionMatch = _captionRegex.Match(followingParagraph?.InnerText ?? string.Empty);
        if (!captionMatch.Success)
        {
            yield return new()
            {
                Code = AnalysisCode.ImageCaption,
                IsError = true,
                ExtraMessage = followingParagraph?.InnerText.TakeFirst(50)
            };

            yield break;
        }

        if (followingParagraph.InnerText.EndsWith('.'))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageCaption,
                IsError = true,
                ExtraMessage = followingParagraph.InnerText.TakeFirst(50)
            };
        }

        var (chapter, order) = RetrieveCaptionInfo(captionMatch);

        if (!HasImageReference(containingParagraph, chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageReference,
                IsError = true,
                ExtraMessage = followingParagraph.InnerText.TakeFirst(50)
            };
        }

        if (!HasImagePredecessor(containingParagraph, chapter, order))
        {
            yield return new()
            {
                Code = AnalysisCode.ImageNumbering,
                IsError = true,
                ExtraMessage = followingParagraph.InnerText.TakeFirst(50)
            };
        }
    }

    private static (int chapter, int order) RetrieveCaptionInfo(Match captionMatch) =>
        (int.Parse(captionMatch.Groups["chapter"].Value), int.Parse(captionMatch.Groups["order"].Value));

    private static bool HasImageReference(OpenXmlElement containingParagraph, int chapter, int order)
    {
        var referenceRegex = new Regex(string.Format(ImageReferencePattern, chapter, order));

        return containingParagraph.TakePreviousSiblingWhile(x => !referenceRegex.IsMatch(x.InnerText)) != null;
    }

    private static bool HasImagePredecessor(OpenXmlElement containingParagraph, int chapter, int order)
    {
        if (order == 1)
        {
            return true;
        }

        var previousNumberRegex = new Regex(string.Format(ImageStartPattern, chapter, order - 1));

        return containingParagraph.TakePreviousSiblingWhile(x => !previousNumberRegex.IsMatch(x.InnerText)) != null;
    }
}
