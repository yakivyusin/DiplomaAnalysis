using DiplomaAnalysis.Services.References;

namespace DiplomaAnalysis;

public class References
{
    private readonly ILogger<References> _logger;

    public References(ILogger<References> logger)
    {
        _logger = logger;
    }

    [Function("References")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new ReferencesService(file.OpenReadStream()), req, _logger);
    }
}
