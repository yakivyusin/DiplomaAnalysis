using DiplomaAnalysis.Services.Runglish;

namespace DiplomaAnalysis;

public class Runglish
{
    private readonly ILogger<Runglish> _logger;

    public Runglish(ILogger<Runglish> logger)
    {
        _logger = logger;
    }

    [Function("Runglish")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new RunglishService(file.OpenReadStream()), req, _logger);
    }
}
