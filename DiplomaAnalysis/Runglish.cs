using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Runglish;

namespace DiplomaAnalysis;

public class Runglish(ILogger<Runglish> logger) : FunctionBase(logger)
{
    [Function("Runglish")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new RunglishService(formFile.OpenReadStream());
}
