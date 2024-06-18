using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Helpers;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly IApplicationApiClient _applicationApiClient;
        protected readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor ) 
        {
            _applicationApiClient = applicationApiClient;
            _contactsApiClient = contactsApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ContactResponse> GetUser()
        {
            var govIdentifier = _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _contactsApiClient.GetContactByGovIdentifier(govIdentifier ?? string.Empty);
        }

        protected async Task<Guid> GetUserId()
        {
            var contact = await GetUser();
            return contact?.Id ?? Guid.Empty;
        }

        protected string GetEpaOrgIdFromClaim()
        {
            return EpaOrgIdFinder.GetFromClaim(_httpContextAccessor);
        }
    }
}