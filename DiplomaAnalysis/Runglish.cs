using DiplomaAnalysis.Services.Runglish;

namespace DiplomaAnalysis
{
    public static class Runglish
    {
        [FunctionName("Runglish")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new RunglishService(file.OpenReadStream()), req, log);
        }
    }
}
