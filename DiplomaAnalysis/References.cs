using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.References;

namespace DiplomaAnalysis;

public class References(ILogger<References> logger) : FunctionBase(logger)
{
    [Function("References")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new ReferencesService(formFile.OpenReadStream());
}
