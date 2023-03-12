using DiplomaAnalysis.Services.References;

namespace DiplomaAnalysis
{
    public static class References
    {
        [FunctionName("References")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new ReferencesService(file.OpenReadStream()), req, log);
        }
    }
}
