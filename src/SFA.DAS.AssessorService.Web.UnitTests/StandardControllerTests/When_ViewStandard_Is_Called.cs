using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ViewStandard_Is_Called : StandardControllerTestBase
    {
        [Test]
        public async Task Then_Use_exisiting_Application_if_exisits()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApiClient
            .Setup(r => r.GetStandardApplications(It.IsAny<Guid>()))
            .ReturnsAsync(new List<ApplicationResponse>()
            {
                new ApplicationResponse() { Id = applicationId, StandardCode = null },
                new ApplicationResponse() { Id = Guid.NewGuid(), StandardCode = 123 }
            });

            _mockContactsApiClient.Setup(r => r.GetContactBySignInId(It.IsAny<String>()))
            .ReturnsAsync(new ContactResponse());

            // Act
            var results = (await _sut.ViewStandard("ST0001")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("ConfirmStandard", results.ActionName);
            Assert.AreEqual(applicationId, results.RouteValues["Id"]);
        }
    }
}
