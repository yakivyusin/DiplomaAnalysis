using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Models;
using DiplomaAnalysis.Services.Table.Helpers;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiplomaAnalysis.Services.Table;

public class TableService : IAnalysisService
{
    private readonly TableRetriever _tableRetriever = new();
    private readonly TableFormattingAnalyzer _tableFormattingAnalyzer = new();
    private readonly TableCaptionAnalyzer _tableCaptionAnalyzer = new();
    private readonly WordprocessingDocument _document;

    public TableService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();

        foreach (var table in _tableRetriever.RetrieveTables(_document))
        {
            result.AddRange(_tableFormattingAnalyzer.Analyze(table));
            result.AddRange(_tableCaptionAnalyzer.Analyze(table));
        }

        return result;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
