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
        const string NOT_FOUND_ID = "00000000-0000-0000-0000-000000000000";

        [SetUp]
        public void TestSetup()
        {
            _mockOrgApiClient
                .Setup(r => r.GetEpaOrganisationById(It.IsNotIn(new string[] { NOT_FOUND_ID })))
                .ReturnsAsync(null as EpaOrganisation);

            List<StandardVersion> versions = new List<StandardVersion> {
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false},
            };

            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ST0001"))
               .ReturnsAsync(versions);

            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ST0002"))
               .ReturnsAsync(versions);

            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ZZ0001"))
               .ReturnsAsync(new List<StandardVersion>());

            _mockContactsApiClient.Setup(r => r.GetContactBySignInId(It.IsAny<String>()))
            .ReturnsAsync(new ContactResponse()
            {
                Username = "USERNAME"
            });
        }

        [Test]
        public async Task Then_Version_Is_Opted_In_Effective_To_Set()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false, StdVersionEffectiveTo = DateTime.Now}
               });

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            // Assert
            _mockOrgApiClient.Verify(m => m.OrganisationStandardVersionOptIn(It.IsAny<Guid>(), It.IsAny<Guid>(), "12345", "ST0001", "1.2", It.IsAny<string>(), true, "Opted in by EPAO by USERNAME"));

            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("OptInConfirmation", results.ActionName);
        }

        [Test]
        public async Task Then_Version_Is_Opted_In_Effective_To_Not_Set()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false, StdVersionEffectiveTo = null}
               });

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            // Assert
            _mockOrgApiClient.Verify(m => m.OrganisationStandardVersionOptIn(It.IsAny<Guid>(), It.IsAny<Guid>(), "12345", "ST0001", "1.2", It.IsAny<string>(), false, "Opted in by EPAO by USERNAME"));

            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("OptInConfirmation", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_Applied_Version_Status_Is_Approved()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false, ApprovedStatus = "Approved"}
               });

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_Applied_Version_Status_Is_Apply_In_Progress()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false, ApprovedStatus = "Apply in progress"}
               });

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_Applied_Version_Status_Is_Feedback_Added()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0001"))
               .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false, ApprovedStatus = "Feedback Added"}
               });

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0001", "1.2")) as RedirectToActionResult;

            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_No_Version_Is_Applied()
        {
            // Arrange
            _mockOrgApiClient
               .Setup(r => r.GetAppliedStandardVersionsForEPAO(It.IsAny<string>(), "ST0002"))
               .ReturnsAsync(new List<AppliedStandardVersion>());

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(Guid.NewGuid(), "ST0002", "1.2")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_No_Version_Exists()
        {
            // Arrange
            _mockOrgApiClient
                .Setup(r => r.GetEpaOrganisationById(NOT_FOUND_ID))
                .ReturnsAsync(null as EpaOrganisation);

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(new Guid(NOT_FOUND_ID), "ZZ0001", "1.2")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }

        [Test]
        public async Task Then_Redirects_If_No_Org_Exists()
        {
            // Arrange
            _mockOrgApiClient
                .Setup(r => r.GetEpaOrganisationById(NOT_FOUND_ID))
                .ReturnsAsync(null as EpaOrganisation);

            // Act
            RedirectToActionResult results = (await _sut.OptInPost(new Guid(NOT_FOUND_ID), "ZZ0001", "1.0")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("Application", results.ControllerName);
            Assert.AreEqual("Applications", results.ActionName);
        }
    }
}
