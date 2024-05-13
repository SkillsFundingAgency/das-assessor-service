using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(IContactsApiClient contactsApiClient, IHttpContextAccessor contextAccessor)
        {
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<ContactResponse> GetUser()
        { 
            var govIdentifier = _contextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _contactsApiClient.GetContactByGovIdentifier(govIdentifier ?? string.Empty);
        }
    }
}
