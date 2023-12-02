using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.CharReplacement;

namespace DiplomaAnalysis;

public class CharReplacement(ILogger<CharReplacement> logger) : FunctionBase(logger)
{
    [Function("CharReplacement")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new CharReplacementService(formFile.OpenReadStream());
}
