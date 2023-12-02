using DiplomaAnalysis.Services.CharReplacement;

namespace DiplomaAnalysis;

public class CharReplacement
{
    private readonly ILogger<CharReplacement> _logger;

    public CharReplacement(ILogger<CharReplacement> logger)
    {
        _logger = logger;
    }

    [Function("CharReplacement")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new CharReplacementService(file.OpenReadStream()), req, _logger);
    }
}
