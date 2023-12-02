using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Punctuation;

namespace DiplomaAnalysis;

public class Punctuation(ILogger<Punctuation> logger) : FunctionBase(logger)
{
    [Function("Punctuation")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new PunctuationService(formFile.OpenReadStream());
}
