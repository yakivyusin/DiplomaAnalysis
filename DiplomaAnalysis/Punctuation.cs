using DiplomaAnalysis.Services.Punctuation;

namespace DiplomaAnalysis
{
    public static class Punctuation
    {
        [FunctionName("Punctuation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new PunctuationService(file.OpenReadStream()), req, log);
        }
    }
}
