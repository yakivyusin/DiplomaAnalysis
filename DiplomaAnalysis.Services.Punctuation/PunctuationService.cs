using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Punctuation;

public sealed class PunctuationService : IAnalysisService
{
    private static readonly Regex[] _punctuationSpacingRegexes =
    [
        new(@"\p{IsCyrillic}[.,:;?!]\p{IsCyrillic}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}\s[.,:;?!]\s\p{IsCyrillic}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}\s[.,:;?!]\p{IsCyrillic}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}[–—]\p{IsCyrillic}", RegexOptions.Compiled)
    ];
    private static readonly Regex _quotesRegex = new(@"(?<!«[^»]+)[“”„""](?![^«]+»)", RegexOptions.Compiled);
    private static readonly Regex[] _typographicRegexes =
    [
        new(@"\.{3}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}\s[-—]\s\p{IsCyrillic}", RegexOptions.Compiled),
        new(@"\P{IsCyrillic}\s[-—]\s\p{IsCyrillic}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}\s[-—]\s\P{IsCyrillic}", RegexOptions.Compiled),
        new(@"\p{IsCyrillic}[`'][яюєї]", RegexOptions.Compiled)
    ];
    private static readonly Regex _multipleWhitespacesRegex = new(@"\s{2,}", RegexOptions.Compiled);
    private readonly WordprocessingDocument _document;

    public PunctuationService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        return _punctuationSpacingRegexes
            .Select(x => (x, AnalysisCode.PunctuationSpacing, isError: true))
            .Append((_quotesRegex, AnalysisCode.Quotes, isError: false))
            .Concat(_typographicRegexes.Select(x => (x, AnalysisCode.Typographical, isError: true)))
            .Append((_multipleWhitespacesRegex, AnalysisCode.MultipleWhitespaces, isError: true))
            .SelectMany(x => Analyze(_document.AllParagraphs(), x))
            .ToList()
            .AsReadOnly();
    }

    private IEnumerable<MessageDto> Analyze(IEnumerable<string> texts, (Regex regex, string code, bool isError) rule)
    {
        return texts
            .SelectMany(x => rule.regex.Matches(x).Select(m => new { Match = m, Text = x }))
            .Select(x => new MessageDto
            {
                Code = rule.code,
                IsError = rule.isError,
                ExtraMessage = x.Match.GetMatchTextWithContext(x.Text, 10)
            });
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
