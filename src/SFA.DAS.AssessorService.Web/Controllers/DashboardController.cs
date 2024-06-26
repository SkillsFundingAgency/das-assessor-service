﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Dashboard;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Api.Common.Exceptions;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class DashboardController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationApiClient;
        private readonly IDashboardApiClient _dashboardApiClient;
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<DashboardController> _logger;

        #region Routes
        public const string DashboardIndexRouteGet = nameof(DashboardIndexRouteGet);
        #endregion

        public DashboardController(
            IHttpContextAccessor contextAccessor,
            IApplicationApiClient applicationApiClient,
            IContactsApiClient contactsApiClient,
            IOrganisationsApiClient organisationApiClient,
            IDashboardApiClient dashboardApiClient,
            IWebConfiguration configuration,
            ILogger<DashboardController> logger)
            :base(applicationApiClient, contactsApiClient, contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _organisationApiClient = organisationApiClient;
            _dashboardApiClient = dashboardApiClient;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("Dashboard", Name = DashboardIndexRouteGet)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> Index()
        {
            var user = await GetUserAndUpdateEmail();
            var organisation = await _organisationApiClient.GetEpaOrganisationById(user?.OrganisationId?.ToString());

            if (user is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if(organisation is null)
            {
                return RedirectToAction("Index", "OrganisationSearch");
            }
            else if(user.Status == ContactStatus.Applying || organisation.Status == OrganisationStatus.Applying)
            {
                return RedirectToAction("Applications", "Application");
            }
            else if (user.EndPointAssessorOrganisationId is null && user.Status == "Invite Pending")
            {
                return RedirectToAction("InvitePending", "Home");
            }
            else if (user.EndPointAssessorOrganisationId is null)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            else if( user.EndPointAssessorOrganisationId != null && user.Status == ContactStatus.Live && organisation.Status != OrganisationStatus.Live)
            {
                return RedirectToAction("NotActivated", "Home");
            }

            var dashboardViewModel = new DashboardViewModel();

            try
            {
                var dashboardResponse = await _dashboardApiClient.GetEpaoDashboard(user.EndPointAssessorOrganisationId);
                dashboardViewModel.StandardsCount = dashboardResponse.StandardsCount;
                dashboardViewModel.PipelinesCount = dashboardResponse.PipelinesCount;
                dashboardViewModel.AssessmentsCount = dashboardResponse.AssessmentsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error getting the dashboard for {user.EndPointAssessorOrganisationId}");
            }

            return View(dashboardViewModel);
        }

        private async Task<ContactResponse> GetUserAndUpdateEmail()
        {
           
            ContactResponse contact = null;
            try
            {
                contact = await GetUser();
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("Failed to retrieve user by Sign In Id.");
            }
            
            if (contact != null)
            {
                var email = _contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == "email")?.Value;
                await _contactsApiClient.UpdateEmail(new UpdateEmailRequest
                {
                    NewEmail = email,
                    GovUkIdentifier = contact.GovUkIdentifier
                });    
            }
            
            return contact;
        }
    }
}