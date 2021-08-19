using DiplomaAnalysis.Services.Orthography2019;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaAnalysis
{
    public static class Orthography2019
    {
        [FunctionName("Orthography2019")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file =>
            {
                using var service = new OrthographyService(file.OpenReadStream());
                return service.Analyze().ToList();
            }, req, log);
        }
    }
}
