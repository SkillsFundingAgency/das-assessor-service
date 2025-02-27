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
    public class When_certificate_already_submitted_for_learner : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate
                    {
                        CertificateReference = "00010001",
                        StandardCode = 12,
                        CertificateData =
                            new CertificateData { OverallGrade = "Distinction" },
                        CertificateLogs = GetCertificateLogs()
                    }
                });

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(It.IsAny<Guid>()))
                .ReturnsAsync(GetCertificateLogs());

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner> {new Domain.Entities.Learner() {StdCode = 12, FamilyName = "Lamora"}});
        }


        [Test]
        public void Then_a_response_is_returned_complete_with_certificate_reference()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() { Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username" },
                    new CancellationToken()).Result;
            result.Count.Should().Be(1);
            result[0].CertificateReference.Should().Be("00010001");
        }

        private List<CertificateLog> GetCertificateLogs()
        {
            return new List<CertificateLog>
            {
                new CertificateLog
                {
                    CertificateData = new CertificateData
                    {
                        OverallGrade = "Distinction",
                        AchievementDate = DateTime.UtcNow.AddDays(-2)
                    },
                    Action = CertificateActions.Submit,
                    EventTime = DateTime.UtcNow.AddDays(-1)
                }
            };
        }
    }
}