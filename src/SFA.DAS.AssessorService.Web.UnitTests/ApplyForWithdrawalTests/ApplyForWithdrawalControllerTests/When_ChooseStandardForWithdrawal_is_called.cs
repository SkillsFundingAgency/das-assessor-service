using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_ChooseStandardForWithdrawal_is_called : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task Then_Standards_Are_Returned()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
                        Id = applicationId,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplicationType = ApplicationTypes.StandardWithdrawal,
                        StandardReference = "ST0001",
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = null }
                        }
                    }
                });

            var registeredStandards = new List<GetEpaoRegisteredStandardsResponse>()
            {
                new GetEpaoRegisteredStandardsResponse() { ReferenceNumber = "ST0001" },
                new GetEpaoRegisteredStandardsResponse() { ReferenceNumber = "ST0002" }
            };

            _mockStandardsApiClient.Setup(m => m.GetEpaoRegisteredStandards(It.IsAny<string>(), 1, 10))
                    .ReturnsAsync(new Domain.Paging.PaginatedList<GetEpaoRegisteredStandardsResponse>(registeredStandards, 2, 1, 3));
                
            // Act
            var result = await _sut.ChooseStandardForWithdrawal(1) as ViewResult;

            // Assert
            var model = result.Model as ChooseStandardForWithdrawalViewModel;
            model.Standards.Items.Should().HaveCount(2);
            model.Standards.Items[0].ApplicationId.Should().Be(applicationId);
            model.Standards.Items[1].ApplicationId.Should().BeNull();
        }
    }
}