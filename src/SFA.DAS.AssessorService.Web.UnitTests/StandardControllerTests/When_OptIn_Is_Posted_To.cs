using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_OptIn_Is_Posted_To : StandardControllerTestBase
    {
        [Test]
        public async Task Then_Version_Is_Opted_In()
        {
            // Arrange
            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ST0001"))
               .ReturnsAsync(new List<StandardVersion> {
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false},
               });

            _mockContactsApiClient.Setup(r => r.GetContactBySignInId(It.IsAny<String>()))
            .ReturnsAsync(new ContactResponse()
            {
                Username = "USERNAME"
            });

            // Act
            var results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            // Assert
            _mockOrgApiClient.Verify(m => m.OrganisationStandardVersionOptIn(It.IsAny<Guid>(), It.IsAny<Guid>(), "12345", "ST0001", "1.2", It.IsAny<string>(), It.IsAny<bool>(), "Opted in by EPAO by USERNAME"));

            Assert.AreEqual("OptInConfirmation", results.ActionName);
        }
    }
}
