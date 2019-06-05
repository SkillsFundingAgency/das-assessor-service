using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers.ManageUsers
{
    [Authorize]
    [CheckSession]
    public class ManageUsersBaseController : Controller
    {
        protected IContactsApiClient ContactsApiClient;
        protected IHttpContextAccessor HttpContextAccessor;

        public ManageUsersBaseController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor)
        {
            ContactsApiClient = contactsApiClient;
            HttpContextAccessor = httpContextAccessor;
        }
        
        protected async Task<(bool isValid, ContactResponse contact)> SecurityCheckAndGetContact(Guid contactId)
        {
            var requestingUser = await ContactsApiClient.GetById(HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value);
            
            var userToBeDisplayed = await ContactsApiClient.GetById(contactId.ToString());
            
            return (requestingUser.OrganisationId == userToBeDisplayed.OrganisationId, userToBeDisplayed);
        }
    }
}