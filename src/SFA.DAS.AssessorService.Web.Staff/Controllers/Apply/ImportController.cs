using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Apply
{
    public class ImportController : Controller
    {
        private readonly IApplyApiClient _apiClient;

        public ImportController(IApplyApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        
        [HttpGet("/Import/UploadWorkflow")]
        public IActionResult UploadWorkflow()
        {
            return View("~/Views/Apply/ImportWorkflow.cshtml");
        }

        [HttpPost("/Import/UploadWorkflow")]
        public async Task<IActionResult> UploadWorkflowFile()
        {
            var formCollection = HttpContext.Request.Form;
            await _apiClient.ImportWorkflow(formCollection.Files.First());
            
            return View("~/Views/Apply/ImportWorkflow.cshtml");
        }
    }
}