using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.Runglish;

public sealed class RunglishService : IAnalysisService
{
    private readonly WordprocessingDocument _document;

    public RunglishService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        return Properties
            .Resources
            .Terms
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(x => Analyze(_document.AllParagraphs(), x))
            .ToList()
            .AsReadOnly();
    }

    private IEnumerable<MessageDto> Analyze(IEnumerable<string> texts, string pattern)
    {
        return texts
            .SelectMany(x => Regex.Matches(x, pattern).Select(m => new { Match = m, Text = x }))
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
