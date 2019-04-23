using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class MigrateUsersController : Controller
    {
        private readonly IContactsApiClient _contacts;

        public MigrateUsersController(IContactsApiClient contacts)
        {
            _contacts = contacts;
        }

        [HttpPost("/MigrateUsers")]
        public async Task<IActionResult> MigrateUsers()
        {
            await _contacts.MigrateUsers();
            return Ok();
        }

        [HttpPost("/MigrateContactsAndOrgsToApply")]
        public async Task<IActionResult> MigrateContactsAndOrgsToApply()
        {
            await _contacts.MigrateContactsAndOrgsToApply();
            return Ok();
        }
    }
}