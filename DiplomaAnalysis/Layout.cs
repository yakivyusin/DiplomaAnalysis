using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Layout;

namespace DiplomaAnalysis;

public class Layout(ILogger<Layout> logger) : FunctionBase(logger)
{
    [Function("Layout")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new LayoutService(formFile.OpenReadStream());
}
