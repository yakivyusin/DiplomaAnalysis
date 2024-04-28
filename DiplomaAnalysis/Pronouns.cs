using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Pronouns;

namespace DiplomaAnalysis;

public class Pronouns(ILogger<Pronouns> logger) : FunctionBase(logger)
{
    [Function("Pronouns")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new PronounsService(formFile.OpenReadStream());
}
