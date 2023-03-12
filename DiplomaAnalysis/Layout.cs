using DiplomaAnalysis.Services.Layout;

namespace DiplomaAnalysis
{
    public static class Layout
    {
        [FunctionName("Layout")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new LayoutService(file.OpenReadStream()), req, log);
        }
    }
}
