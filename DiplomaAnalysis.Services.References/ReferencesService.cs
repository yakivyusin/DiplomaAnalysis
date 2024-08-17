using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DiplomaAnalysis.Services.References.Helpers;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.References;

public sealed class ReferencesService : IAnalysisService
{
    private static readonly Regex ApaReferenceRegex = new(@"\(\p{L}.+, \d{4}[a-z]?\)", RegexOptions.Compiled);

    private readonly DstuReferencesAnalyzer _dstuReferencesAnalyzer = new();
    private readonly WordprocessingDocument _document;

    public ReferencesService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();

        var (areDstuReferencesExist, areDstuReferencesInCorrectOrder) = _dstuReferencesAnalyzer.Analyze(_document);

        var apaReferences = _document
            .AllParagraphs()
            .SelectMany(x => ApaReferenceRegex.Matches(x))
            .ToList();

        if (!areDstuReferencesExist && apaReferences.Count == 0)
        {
            result.Add(new()
            {
                Code = AnalysisCode.References,
                IsError = false,
                ExtraMessage = null
            });
        }

        if (!areDstuReferencesInCorrectOrder)
        {
            result.Add(new()
            {
                Code = AnalysisCode.ReferencesOrder,
                IsError = true,
                ExtraMessage = null
            });
        }

        return result;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
