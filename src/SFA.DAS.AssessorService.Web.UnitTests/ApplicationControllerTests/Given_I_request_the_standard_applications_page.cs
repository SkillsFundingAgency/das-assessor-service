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
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.AssessorService.Web.UnitTests.AppplicationControllerTests
{
    public class Given_I_request_the_standard_applications_page
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
                        new("http://schemas.portal.com/epaoid","anything")
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
            _organisationsApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).Returns(Task.FromResult(
                new Api.Types.Models.AO.EpaOrganisation 
                { 
                    FinancialReviewStatus = FinancialReviewStatus.Approved
                }
                ));

            _applicationApiClient = new Mock<IApplicationApiClient>();
            _applicationApiClient.Setup(x => x.GetStandardApplications(It.IsAny<Guid>())).Returns(Task.FromResult(
               new List<ApplicationResponse>
               {
                    new ApplicationResponse{ ApplicationId = new Guid(), ApplicationStatus = ApplicationStatus.New}
               }
                ));

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);

            mockUrlHelper
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("callbackUrl")
                .Verifiable();


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
            _controller.Url = mockUrlHelper.Object;

        }


        [Test]
        public async Task When_financial_status_not_exempt_start_or_resume()
        {

            var actual = await _controller.StandardApplications();

            var result = actual as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as ApplicationResponseViewModel;
            Assert.IsNotNull(model);

            model.FinancialAssessmentUrl.Should().Be("callbackUrl");
        }

        [Test]
        public async Task When_financial_status_exempt_show_application()
        {
            _organisationsApiClient.Setup(x => x.GetEpaOrganisation(It.IsAny<string>())).Returns(Task.FromResult(
                new Api.Types.Models.AO.EpaOrganisation
                {
                    FinancialReviewStatus = FinancialReviewStatus.Exempt
                }
                ));

            var actual = await _controller.StandardApplications();

            var result = actual as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as ApplicationResponseViewModel;
            Assert.IsNotNull(model);

            model.FinancialAssessmentUrl.Should().Be(string.Empty);
        }
    }    
}

