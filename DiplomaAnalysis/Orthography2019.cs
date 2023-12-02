using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Orthography2019;

namespace DiplomaAnalysis;

public class Orthography2019(ILogger<Orthography2019> logger) : FunctionBase(logger)
{
    [Function("Orthography2019")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new OrthographyService(formFile.OpenReadStream());
}
