using DiplomaAnalysis.Services.Orthography2019;

namespace DiplomaAnalysis
{
    public static class Orthography2019
    {
        [FunctionName("Orthography2019")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file => new OrthographyService(file.OpenReadStream()), req, log);
        }
    }
}
