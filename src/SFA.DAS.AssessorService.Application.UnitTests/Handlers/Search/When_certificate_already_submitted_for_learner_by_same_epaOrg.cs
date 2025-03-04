using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_certificate_already_submitted_for_learner_by_same_epaOrg : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateId = Guid.NewGuid();

            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
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
                                OverallGrade = CertificateGrade.Distinction,
                                LearningStartDate = new DateTime(2015, 06, 01),
                                AchievementDate = new DateTime(2018, 06, 01)
                            }),
                        CreatedBy = "username",
                        CertificateLogs = new List<CertificateLog> 
                        { 
                            new CertificateLog()
                            {
                                CertificateData = JsonConvert.SerializeObject(new CertificateData
                                {
                                    OverallGrade = CertificateGrade.Distinction,
                                    LearningStartDate = new DateTime(2015, 06, 01),
                                    AchievementDate = new DateTime(2018, 06, 01)
                                }),
                                Status = CertificateStatus.Submitted,
                                EventTime = new DateTime(2018, 2, 3, 13, 23, 33),
                                Username = "username",
                                Action = CertificateActions.Submit
                            } 
                        },
                    }
                });

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(certificateId))
                .ReturnsAsync(new List<CertificateLog>()
                {
                    new CertificateLog()
                    {
                        Status = CertificateStatus.Submitted,
                        EventTime = new DateTime(2018, 2, 3, 13, 23, 33),
                        Username = "username",
                        Action = CertificateActions.Submit,
                    },
                    new CertificateLog()
                    {
                        Status = CertificateStatus.Draft,
                        EventTime = new DateTime(2018, 2, 3, 13, 23, 32),
                        Username = "username",
                        Action = CertificateActions.Start
                    }
                });

            ContactRepository.Setup(cr => cr.GetContact("username"))
                .ReturnsAsync(new Contact() {DisplayName = "EPAO User from same EAPOrg"});

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner> {new Domain.Entities.Learner() {StdCode = 12, FamilyName = "Lamora"}});
        }


        [Test]
        public void Then_a_response_is_returned_with_sensitive_fields_populated()
        {
            var result =
                SearchHandler.Handle(
                    new CertificateSearchRequest() {Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;

            result[0].SubmittedAt.Should().Be(new DateTime(2018,2,3,13,23,33));
            result[0].SubmittedBy.Should().Be("EPAO User from same EAPOrg");
            result[0].LearnStartDate.Should().Be(new DateTime(2015,06,01));
            result[0].AchDate.Should().Be(new DateTime(2018,06,01));
            result[0].OverallGrade.Should().Be("Distinction");
            result[0].ShowExtraInfo.Should().BeTrue();
        }
    }
}