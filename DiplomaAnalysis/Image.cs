using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Image;

namespace DiplomaAnalysis;

public class Image(ILogger<Image> logger) : FunctionBase(logger)
{
    [Function("Image")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new ImageService(formFile.OpenReadStream());
}
