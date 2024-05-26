using DiplomaAnalysis.Common.Contracts;
using DiplomaAnalysis.Services.Table;

namespace DiplomaAnalysis;

public class Table(ILogger<Table> logger) : FunctionBase(logger)
{
    [Function("Table")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return RunAnalysis(req);
    }

    protected override IAnalysisService CreateService(IFormFile formFile) => new TableService(formFile.OpenReadStream());
}
