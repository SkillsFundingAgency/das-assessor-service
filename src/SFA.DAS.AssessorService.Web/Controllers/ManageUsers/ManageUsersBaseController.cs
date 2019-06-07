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
        protected ContactResponse RequestingUser;
        protected ContactResponse UserToBeDisplayed;

        public ManageUsersBaseController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor)
        {
            ContactsApiClient = contactsApiClient;
            HttpContextAccessor = httpContextAccessor;
        }
        
        protected async Task<(bool isValid, ContactResponse contact)> SecurityCheckAndGetContact(Guid contactId)
        {
            RequestingUser = await GetRequestingContact();
            
            UserToBeDisplayed = await ContactsApiClient.GetById(contactId);
            
            return (RequestingUser.OrganisationId == UserToBeDisplayed.OrganisationId, UserToBeDisplayed);
        }

        protected async Task<ContactResponse> GetRequestingContact()
        {
            return await ContactsApiClient.GetById(Guid.Parse(HttpContextAccessor.HttpContext.User.FindFirst("UserId").Value));
        }
    }
}