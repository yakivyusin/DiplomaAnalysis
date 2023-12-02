using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.WordingMisuse;

namespace DiplomaAnalysis;

public class WordingMisuse(ILogger<WordingMisuse> logger) : FunctionBase(logger)
{
    [Function("WordingMisuse")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new WordingMisuseService(formFile.OpenReadStream());
}
