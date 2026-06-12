using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    public class ApplicationFinderTests
    {
        [TestCase(ApplicationStatus.InProgress)]
        [TestCase(ApplicationStatus.FeedbackAdded)]
        [TestCase(ApplicationStatus.Resubmitted)]
        [TestCase(ApplicationStatus.InProgress)]
        public async Task GetApplicationResponse_ReturnsApplications_WithSpecificApplicationStatus(string applicationStatus)
        {
            var contactId = Guid.NewGuid();
            var applicationResponses = GetApplicationResponsesWithOneDesiredItem(applicationStatus);

            var mockApiClient = new Mock<IApplicationApiClient>();

            mockApiClient.Setup(m => m.GetWithdrawalApplications(contactId))
                         .ReturnsAsync(applicationResponses);

            var sut = new ApplicationFinder(mockApiClient.Object);
            var applications = await sut.GetWithdrawalApplications(contactId);

            var result = applications.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.StandardReference) &&
                                               a.StandardReference.Equals("ST9999", StringComparison.InvariantCultureIgnoreCase) &&
                                               a.ApplyData.Apply.Versions == null);

            result.Should().NotBeNull();
            result.StandardReference.Should().Be("ST9999");
            result.ApplicationStatus.Should().Be(applicationStatus);

        }

        private List<ApplicationResponse> GetApplicationResponsesWithOneDesiredItem(string applicationStatus)
        {
            var result = new List<ApplicationResponse>
            {
                new ApplicationResponse() { ApplyData = GetApplyDataWithNullVersion(), StandardReference = "ST0001", ApplicationStatus = ApplicationStatus.Approved },
                new ApplicationResponse() { ApplyData = GetApplyDataWithNullVersion(), StandardReference = "ST0002", ApplicationStatus = ApplicationStatus.Declined },
                new ApplicationResponse() { ApplyData = GetApplyDataWithNullVersion(), StandardReference = "ST9999", ApplicationStatus = applicationStatus },
                new ApplicationResponse() { ApplyData = GetApplyDataWithNullVersion(), StandardReference = "ST0003", ApplicationStatus = ApplicationStatus.Deleted },
                new ApplicationResponse() { ApplyData = GetApplyDataWithNullVersion(), StandardReference = "ST0004", ApplicationStatus = ApplicationStatus.New }
            };
            result.ForEach(r => r.ApplyData = GetApplyDataWithNullVersion());
            result.ForEach(r => r.ApplicationType = ApplicationTypes.StandardWithdrawal);

            return result;
        }

        private ApplyData GetApplyDataWithNullVersion()
        {
            var data = new ApplyData();
            data.Apply = new ApplyInfo() { Versions = null };
            return data;
        }
    }
}