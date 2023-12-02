using DiplomaAnalysis.Services.Orthography2019;

namespace DiplomaAnalysis;

public class Orthography2019
{
    private readonly ILogger<Orthography2019> _logger;

    public Orthography2019(ILogger<Orthography2019> logger)
    {
        _logger = logger;
    }

    [Function("Orthography2019")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new OrthographyService(file.OpenReadStream()), req, _logger);
    }
}
