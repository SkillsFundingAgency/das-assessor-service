using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_certificate_already_submitted_for_learner_by_different_epaOrg : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateId = Guid.NewGuid();

            var submittingEpaoOrgId = Guid.NewGuid();
            var searchingEpaoOrgId = Guid.NewGuid();

            CertificateRepository.Setup(r => r.GetCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate
                    {
                        Id = certificateId,
                        CertificateReference = "00010001",
                        StandardCode = 12,
                        CertificateData =
                            JsonConvert.SerializeObject(new CertificateData
                            {
                                OverallGrade = "Distinction",
                                LearningStartDate = new DateTime(2015, 06, 01),
                                AchievementDate = new DateTime(2018, 06, 01)
                            }),
                        CreatedBy = "differentuser"
                    }
                });

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(certificateId))
                .ReturnsAsync(new List<CertificateLog>()
                {
                    new CertificateLog()
                    {
                        Status = CertificateStatus.Submitted, EventTime = new DateTime(2018, 2, 3, 13, 23, 33),
                        Username = "differentuser"
                    },
                    new CertificateLog()
                    {
                        Status = CertificateStatus.Draft, EventTime = new DateTime(2018, 2, 3, 13, 23, 32),
                        Username = "differentuser"
                    }
                });

            ContactRepository.Setup(cr => cr.GetContact("differentuser"))
                .ReturnsAsync(new Contact()
                    {DisplayName = "EPAO User from another EAPOrg", OrganisationId = submittingEpaoOrgId});

            ContactRepository.Setup(cr => cr.GetContact("username"))
                .ReturnsAsync(new Contact()
                    {DisplayName = "EPAO User from this EAPOrg", OrganisationId = searchingEpaoOrgId});


            IlrRepository.Setup(r => r.SearchForLearnerByUlnAndFamilyName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr()
                    {
                        Id = Guid.NewGuid(), StdCode = 12, FamilyName = "Lamora", Uln = 1111111111,
                        CompletionStatus = CompletionStatus.Completed
                    }
                });

            IlrRepository.Setup(r => r.RefreshFromSubmissionEventData(It.IsAny<Guid>(), It.IsAny<SubmissionEvent>()))
                .Returns(Task.CompletedTask);
            IlrRepository.Setup(r => r.MarkAllUpToDate(It.IsAny<long>())).Returns(Task.CompletedTask);

        }


        [Test]
        public void Then_a_response_is_returned_with_sensitive_fields_empty()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", UkPrn = 12345, Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;

            result[0].SubmittedAt.Should().BeNull();
            result[0].SubmittedBy.Should().BeNullOrEmpty();
            result[0].AchDate.Should().BeNull();//(new DateTime(2018, 06, 01))
            result[0].OverallGrade.Should().BeNullOrEmpty();
            result[0].ShowExtraInfo.Should().BeFalse();
        }
    }
}