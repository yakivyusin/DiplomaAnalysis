using DiplomaAnalysis.Services.WordingMisuse;

namespace DiplomaAnalysis;

public class WordingMisuse
{
    private readonly ILogger<WordingMisuse> _logger;

    public WordingMisuse(ILogger<WordingMisuse> logger)
    {
        _logger = logger;
    }

    [Function("WordingMisuse")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new WordingMisuseService(file.OpenReadStream()), req, _logger);
    }
}
