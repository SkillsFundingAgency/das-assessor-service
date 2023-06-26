using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class AssessorController : Controller
    {
        protected readonly IApplicationApiClient _applicationApiClient;
        protected readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssessorController(IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _applicationApiClient = applicationApiClient;
            _contactsApiClient = contactsApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<Guid> GetUserId()
        {
            var contact = await GetUserContact();
            return contact?.Id ?? Guid.Empty;
        }

        protected string GetEpaOrgIdFromClaim()
        {
            var epaoid = _httpContextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            return epaoid;
        }

        protected async Task<ContactResponse> GetUserContact()
        {
            var signinId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            return await GetUserContact(signinId);
        }

        protected string GetEpaOrgIdFromClaim()
        {
            var epaoid = _httpContextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            return epaoid;
        }

        private async Task<ContactResponse> GetUserContact(string signinId)
        {
            return await _contactsApiClient.GetContactBySignInId(signinId);
        }
    }
}