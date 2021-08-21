using DiplomaAnalysis.Services.WordingMisuse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaAnalysis
{
    public static class WordingMisuse
    {
        [FunctionName("WordingMisuse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await RunService.Run(file =>
            {
                using var service = new WordingMisuseService(file.OpenReadStream());
                return service.Analyze().ToList();
            }, req, log);
        }
    }
}
