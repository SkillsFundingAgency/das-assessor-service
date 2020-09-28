using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public AssessorController(IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _contactsApiClient = contactsApiClient;
        }

        protected async Task<Guid> GetUserId()
        {
            var contact = await GetUserContact();
            return contact?.Id ?? Guid.Empty;
        }

        protected async Task<ContactResponse> GetUserContact()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            return await GetUserContact(signinId);
        }

        private async Task<ContactResponse> GetUserContact(string signinId)
        {
            return await _contactsApiClient.GetContactBySignInId(signinId);
        }
    }
}