﻿using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Extensions;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Services.References;

public sealed class ReferencesService : IAnalysisService
{
    private static readonly Regex DstuReferenceRegex = new(@"\[(\d+)\]", RegexOptions.Compiled);
    private static readonly Regex ApaReferenceRegex = new(@"\(\p{L}.+, \d{4}[a-z]?\)", RegexOptions.Compiled);
    
    private readonly WordprocessingDocument _document;

    public ReferencesService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();

        var dstuReferences = _document
            .AllParagraphs()
            .SelectMany(x => DstuReferenceRegex.Matches(x))
            .ToList();

        var apaReferences = _document
            .AllParagraphs()
            .SelectMany(x => ApaReferenceRegex.Matches(x))
            .ToList();

        if (dstuReferences.Count == 0 && apaReferences.Count == 0)
        {
            result.Add(new()
            {
                Code = AnalysisCode.References,
                IsError = false,
                ExtraMessage = null
            });
        }

        if (dstuReferences.Count > 0 && !AreDstuInCorrectOrder(dstuReferences))
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

    private bool AreDstuInCorrectOrder(List<Match> dstuReferences)
    {
        var numbers = dstuReferences
            .Select(x => int.Parse(x.Groups[1].Value))
            .ToList();

        if (numbers[0] != 1)
        {
            return false;
        }

        foreach (var (number, index) in numbers.Select((x, index) => (number: x, index)).Where(x => x.number > 1))
        {
            if (!numbers.Take(index).Any(x => x == number - 1))
            {
                return false;
            }
        }

        return true;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
