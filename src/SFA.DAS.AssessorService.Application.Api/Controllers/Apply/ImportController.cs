using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
{
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IWebConfiguration _config;

        public ImportController(ILogger<ImportController> logger, IWebConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost("/Import/Workflow")]
        public async Task<ActionResult> ImportWorkflow()
        {
            var file = HttpContext.Request.Form.Files.First();
            using (var httpClient = new HttpClient(){BaseAddress = new Uri(_config.ApplyApiAuthentication.ApiBaseAddress)})
            {
                var formDataContent = new MultipartFormDataContent();
                
                var fileContent = new StreamContent(file.OpenReadStream())
                    {Headers = {ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)}};
                formDataContent.Add(fileContent, file.Name, file.FileName);
                
                await httpClient.PostAsync($"/Import/Workflow", formDataContent);
            }

            return Ok();
        }
    }
}