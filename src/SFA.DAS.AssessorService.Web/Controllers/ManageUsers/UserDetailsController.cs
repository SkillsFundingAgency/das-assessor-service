using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    [CheckSession]
    public class UserDetailsController : Controller
    {
        [HttpGet("/ManageUsers/{userid}")]
        public async Task<IActionResult> User(Guid userid)
        {
            return View();
        }
    }
}