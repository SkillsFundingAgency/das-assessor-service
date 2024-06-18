using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using ApplicationController = SFA.DAS.AssessorService.Web.Controllers.Apply.ApplicationController;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.UnitTests.AppplicationControllerTests
{
    public class Given_I_request_the_applications_page
    {
        private ApplicationController _controller;
        private Mock<IContactsApiClient> _contactsApiClient;
        private Mock<IOrganisationsApiClient> _organisationsApiClient;
        private Mock<IApplicationApiClient> _applicationApiClient;
        private const string Email = "test@test.com";
        private const string GovIdentifier = "GovIdentifier-12345";
       
        [SetUp]
        public void Arrange()
        {
            _contactsApiClient = new Mock<IContactsApiClient>();

            _contactsApiClient
                .Setup(x => x.GetContactByGovIdentifier(It.IsAny<string>())).Returns(Task.FromResult(
                new ContactResponse
                {
                    Username = "Unknown-100",
                    Email = Email,
                    GovUkIdentifier = GovIdentifier
                }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new ( new List<Claim>
                    {
                        new(ClaimTypes.Email, Email),
                        new(ClaimTypes.NameIdentifier, GovIdentifier),
                    })
                })
            });

            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = true,
                }
                ));
            _applicationApiClient = new Mock<IApplicationApiClient>();
           

            _controller = new ApplicationController(
                Mock.Of<IApiValidationService>(),
                Mock.Of<IApplicationService>(),
                _organisationsApiClient.Object,
                Mock.Of<IQnaApiClient>(),
                Mock.Of<IRegisterApiClient>(),
                new WebConfiguration(),
                _applicationApiClient.Object,
                Mock.Of<IContactsApiClient>(),
                httpContextAccessor.Object,
                Mock.Of<ILogger<ApplicationController>>());

            _controller.TempData = new TempDataDictionary(httpContextAccessor.Object.HttpContext, Mock.Of<ITempDataProvider>());

        }


        [Test]
        public async Task When_no_applications_and_ROEPAOApproved_is_true()
        {
            var actual = await _controller.Applications();

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.ActionName.Should().Be("StandardApplications");
        }

        [Test]
        public async Task When_no_applications_and_ROEPAOApproved_is_false()
        {
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            var actual = await _controller.Applications();

            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.ViewName.Should().Be("~/Views/Application/Declaration.cshtml");
        }

        [Test]
        public async Task When_single_application_and_ROEPAOApproved_is_true()
        {
            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.New}
                }
            ));

            var actual = await _controller.Applications();

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.ActionName.Should().Be("StandardApplications");
        }

        [Test]
        public async Task When_multiple_applications_show_StandardApplication()
        {
            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.New},
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.Approved}
                }
            ));

            var actual = await _controller.Applications();

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);

            actualViewResult.ActionName.Should().Be("StandardApplications");
        }

        [Test]
        public async Task When_application_status_is_feedback_added_show_feedback_intro()
        {
            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.FeedbackAdded}
                }
            ));

            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            var actual = await _controller.Applications();

            var result = actual as ViewResult;
            Assert.IsNotNull(result);

            result.ViewName.Should().Be("~/Views/Application/FeedbackIntro.cshtml");
            result.Model.Should().BeOfType<FeedbackIntroViewModel>();
        }

        [Test]
        public async Task When_application_status_is_approved_show_standard_application()
        {
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.Approved}
                }
            ));

            var actual = await _controller.Applications();

            var result = actual as RedirectToActionResult;
            Assert.IsNotNull(result);

            result.ActionName.Should().Be("StandardApplications");
        }

        [Test]
        public async Task When_application_status_is_submitted_show_standard_application()
        {
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.Submitted}
                }
            ));

            var actual = await _controller.Applications();

            var result = actual as RedirectToActionResult;
            Assert.IsNotNull(result);

            result.ActionName.Should().Be("Submitted");
        }

        [Test]
        public async Task When_application_status_is_resubmitted_show_standard_application()
        {
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.Resubmitted}
                }
            ));

            var actual = await _controller.Applications();

            var result = actual as RedirectToActionResult;
            Assert.IsNotNull(result);

            result.ActionName.Should().Be("Submitted");
        }

        [Test]
        public async Task When_application_status_is_in_progress_show_SequenceSignPost()
        {
            _organisationsApiClient.Setup(x => x.GetOrganisationByUserId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new OrganisationResponse
                {
                    Id = new Guid(),
                    RoEPAOApproved = false,
                }
                ));

            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
                new List<ApplicationResponse>
                {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.InProgress}
                }
            ));

            var actual = await _controller.Applications();

            var result = actual as RedirectToActionResult;
            Assert.IsNotNull(result);

            result.ActionName.Should().Be("SequenceSignPost");
        }

    }    
}

