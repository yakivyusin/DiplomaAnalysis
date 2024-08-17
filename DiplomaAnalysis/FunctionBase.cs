using DiplomaAnalysis.Common.Contracts;

namespace DiplomaAnalysis;

public abstract class FunctionBase
{
    private readonly ILogger _logger;

    protected FunctionBase(ILogger logger)
    {
        _logger = logger;
    }

    protected IActionResult RunAnalysis(HttpRequest request)
    {
        if (request.Form.Files.Count == 0)
        {
            _logger.LogInformation("Request without files");

            return new BadRequestResult();
        }

        try
        {
            using var service = CreateService(request.Form.Files[0]);

            var result = service.Analyze();

            return new OkObjectResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Incoming file parsing error");

            return new BadRequestResult();
        }
    }

    protected abstract IAnalysisService CreateService(IFormFile formFile);
}
