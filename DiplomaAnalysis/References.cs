using DiplomaAnalysis.Services.References;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DiplomaAnalysis
{
    public static class References
    {
        [FunctionName("References")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (req.Form.Files.Count == 0)
            {
                log.LogInformation("Request without files");
                return new BadRequestResult();
            }

            var file = req.Form.Files[0];

            try
            {
                using var service = new ReferencesService(file.OpenReadStream());
                var result = service.Analyze();

                return new OkObjectResult(result);
            }
            catch
            {
                log.LogWarning("Incoming file parsing error");
                return new BadRequestResult();
            }
        }
    }
}
