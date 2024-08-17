using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace DiplomaAnalysis.Services.Image.Helpers;

internal class ImageRetriever
{
    public IEnumerable<Drawing> RetrieveImages(WordprocessingDocument document) => document.MainDocumentPart
        .Document
        .Body
        .Descendants<Drawing>();
}
