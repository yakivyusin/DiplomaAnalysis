using DiplomaAnalysis.Services.Layout;

namespace DiplomaAnalysis;

public class Layout
{
    private readonly ILogger<Layout> _logger;

    public Layout(ILogger<Layout> logger)
    {
        _logger = logger;
    }

    [Function("Layout")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        return await RunService.Run(file => new LayoutService(file.OpenReadStream()), req, _logger);
    }
}
