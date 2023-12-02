using DiplomaAnalysis.Common.Contracts;

namespace DiplomaAnalysis;

public static class RunService
{
    public static async Task<IActionResult> Run(
        Func<IFormFile, IAnalysisService> factoryFunc,
        HttpRequest req,
        ILogger log)
    {
        if (req.Form.Files.Count == 0)
        {
            log.LogInformation("Request without files");

            return new BadRequestResult();
        }

        try
        {
            using var service = factoryFunc(req.Form.Files[0]);

            var result = service.Analyze();

            return new OkObjectResult(result);
        }
        catch
        {
            log.LogWarning("Incoming file parsing error");

            return new BadRequestResult();
        }
    }
}
