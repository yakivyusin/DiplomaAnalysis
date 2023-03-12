using DiplomaAnalysis.Services.WordingMisuse;

namespace DiplomaAnalysis
{
    public static class WordingMisuse
    {
        [FunctionName("WordingMisuse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new WordingMisuseService(file.OpenReadStream()), req, log);
        }
    }
}
