using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using SFA.DAS.AssessorService.Web.Controllers.OppFinder;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class OppFinderExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public OppFinderExceptionFilterAttribute(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                return;
            }

            context.Result = new RedirectToActionResult(nameof(OppFinderController.Error), nameof(OppFinderController).RemoveController(), new { });
            context.ExceptionHandled = true;
        }
    }
}
