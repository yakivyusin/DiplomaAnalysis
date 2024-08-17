using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Models;
using DiplomaAnalysis.Services.Image.Helpers;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiplomaAnalysis.Services.Image;

public class ImageService : IAnalysisService
{
    private readonly ImageRetriever _imageRetriever = new();
    private readonly ImageFormattingAnalyzer _imageFormattingAnalyzer = new();
    private readonly ImageCaptionAnalyzer _imageCaptionAnalyzer = new();
    private readonly WordprocessingDocument _document;

    public ImageService(Stream data)
    {
        _document = WordprocessingDocument.Open(data, false);
    }

    public IReadOnlyCollection<MessageDto> Analyze()
    {
        var result = new List<MessageDto>();

        foreach (var table in _imageRetriever.RetrieveImages(_document))
        {
            result.AddRange(_imageFormattingAnalyzer.Analyze(table));
            result.AddRange(_imageCaptionAnalyzer.Analyze(table));
        }

        return result;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
