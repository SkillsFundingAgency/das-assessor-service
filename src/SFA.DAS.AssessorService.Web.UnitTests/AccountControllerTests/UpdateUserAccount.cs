using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    public class UpdateUserAccount
    {
        private AccountController _controller;
        private Mock<UpdateAccountValidator> _validator;
        private Mock<IContactsApiClient> _contactsApiClient;
        private const string Email = "test@test.com";
        private const string GovIdentifier = "GovIdentifier-12345";
        private const string GivenName = "Your";
        private const string FamilyName = "Name";

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<UpdateAccountValidator>();
            _contactsApiClient = new Mock<IContactsApiClient>();


            _contactsApiClient
                .Setup(x => x.InviteUser(It.Is<CreateContactRequest>(c =>
                    c.Email.Equals(Email) && c.GivenName.Equals(GivenName) && c.FamilyName.Equals(FamilyName))))
                .ReturnsAsync(new ContactBoolResponse(true));
            
            
            _controller = new AccountController(Mock.Of<ILogger<AccountController>>(), Mock.Of<ILoginOrchestrator>(),
                Mock.Of<ISessionService>(), new WebConfiguration(), _contactsApiClient.Object,
                Mock.Of<IHttpContextAccessor>(), Mock.Of<CreateAccountValidator>(), Mock.Of<IOrganisationsApiClient>(), _validator.Object, null);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new List<ClaimsIdentity>
                    {
                        new ( new List<Claim>
                        {
                            new(ClaimTypes.Email, Email),
                            new(ClaimTypes.NameIdentifier, GovIdentifier),
                        })
                    })
                }
            };
        }

        [Test]
        public async Task Then_If_Not_Valid_View_Returned()
        {
            var accountViewModel = new AccountViewModel();
            _controller.ModelState.AddModelError("error","error");
            
            var actual = await _controller.UpdateAnAccount(accountViewModel);

            var actualViewResult = actual as ViewResult;
            _validator.Verify(x =>
                x.ValidateAsync(
                    It.Is<ValidationContext<AccountViewModel>>(c => c.InstanceToValidate.Equals(accountViewModel)),
                    CancellationToken.None), Times.Once);
            Assert.IsNotNull(actualViewResult);
        }
    
        [Test]
        public async Task Then_If_Valid_Calls_Invite_User_With_Gov_Claims_Redirects_To_Invite_Sent()
        {
            var accountViewModel = new AccountViewModel
            {
                FamilyName = FamilyName,
                GivenName = GivenName
            };
            var actual = await _controller.UpdateAnAccount(accountViewModel);

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);
            _validator.Verify(x =>
                x.ValidateAsync(
                    It.Is<ValidationContext<AccountViewModel>>(c => c.InstanceToValidate.Equals(accountViewModel)),
                    CancellationToken.None), Times.Once);
            Assert.AreEqual("InviteSent",actualViewResult.ActionName);
        }

        [Test]
        public async Task Then_If_Valid_Call_But_Fails_To_Create_Invite_Shows_Error()
        {
            _contactsApiClient
                .Setup(x => x.InviteUser(It.Is<CreateContactRequest>(c =>
                    c.Email.Equals(Email) && c.GivenName.Equals(GivenName) && c.FamilyName.Equals(FamilyName) && c.GovIdentifier.Equals(GovIdentifier))))
                .ReturnsAsync(new ContactBoolResponse(false));
            var accountViewModel = new AccountViewModel
            {
                FamilyName = FamilyName,
                GivenName = GivenName
            };
            var actual = await _controller.UpdateAnAccount(accountViewModel);

            var actualViewResult = actual as RedirectToActionResult;
            Assert.IsNotNull(actualViewResult);
            _validator.Verify(x =>
                x.ValidateAsync(It.Is<ValidationContext<AccountViewModel>>(c => c.InstanceToValidate.Equals(accountViewModel)),
                    CancellationToken.None), Times.Once);
            Assert.AreEqual("Error",actualViewResult.ActionName);
        }
        
    }    
}

