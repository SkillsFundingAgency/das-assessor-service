namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = Domain.Roles.RoatpGatewayTeam)]
    public class RoatpHomeController : Controller
    {
        [Route("search-apprenticeship-training-providers")]
        public IActionResult Index()
        {           
            return View("~/Views/Roatp/Index.cshtml");
        }
    }
}
