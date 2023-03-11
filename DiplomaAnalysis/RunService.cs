using DiplomaAnalysis.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaAnalysis
{
    public static class RunService
    {
        public static async Task<IActionResult> Run(
            Func<IFormFile, IEnumerable<MessageDto>> analysisFunc,
            HttpRequest req,
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
                var result = analysisFunc(file).ToList();

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
