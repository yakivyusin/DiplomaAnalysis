using DiplomaAnalysis.Services.Punctuation;

namespace DiplomaAnalysis;

public class Punctuation
{
    private readonly ILogger<Punctuation> _logger;

    public Punctuation(ILogger<Punctuation> logger)
    {
        _logger = logger;
    }

    [Function("Punctuation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new PunctuationService(file.OpenReadStream()), req, _logger);
    }
}
