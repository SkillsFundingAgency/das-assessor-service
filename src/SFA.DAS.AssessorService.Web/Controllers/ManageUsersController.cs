using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class ManageUsersController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IEmailApiClient _emailApiClient;
        private readonly IWebConfiguration _config;

        public ManageUsersController(IWebConfiguration config, IContactsApiClient contactsApiClient,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient, IEmailApiClient emailApiClient)
        {
            _contactsApiClient = contactsApiClient;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _emailApiClient = emailApiClient;
            _config = config;
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> Index()
        {
            var response = new List<ContactsWithPrivilegesResponse>();
            try
            {
                var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
                var organisation = await _organisationsApiClient.Get(ukprn);
                if (organisation != null)
                 response = await _contactsApiClient.GetContactsWithPrivileges(organisation.EndPointAssessorOrganisationId);
               
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            
           
            return View(response);
        }

        [HttpGet]
        [Route("/[controller]/status/{id}/{status}")]
        public async Task<IActionResult> SetStatusAndNotify(string id, string status)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(status))
            {
                await _contactsApiClient.UpdateStatus(new UpdateContactStatusRequest(id, status));
                if (status == ContactStatus.Approve)
                {
                    var contactResponse = await _contactsApiClient.GetById(id);
                    var emailTemplate =
                        await _emailApiClient.GetEmailTemplate(EmailTemplateName.AssessorEpaoApproveConfirm);
                    await _emailApiClient.SendEmailWithTemplate(new SendEmailRequest(contactResponse.Email,
                        emailTemplate, new
                        {
                            contactname = $"{contactResponse.DisplayName}",
                            ServiceLink = _config.ServiceLink
                        }));
                }
            }
            return  RedirectToAction("Index");
        }
            
    }
}