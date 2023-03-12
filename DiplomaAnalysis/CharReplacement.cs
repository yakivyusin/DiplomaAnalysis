using DiplomaAnalysis.Services.CharReplacement;

namespace DiplomaAnalysis
{
    public static class CharReplacement
    {
        [FunctionName("CharReplacement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new CharReplacementService(file.OpenReadStream()), req, log);
        }
    }
}
