using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    [CheckSession]
    public class InviteUserController : Controller
    {
        public InviteUserController()
        {
            
        }

        [HttpGet("/ManageUsers/Invite")]
        public async Task<IActionResult> Invite()
        {
            return View();
        }
    }
}