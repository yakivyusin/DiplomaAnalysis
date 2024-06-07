using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;

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

        return result;
    }

    public void Dispose()
    {
        ((IDisposable)_document).Dispose();
    }
}
