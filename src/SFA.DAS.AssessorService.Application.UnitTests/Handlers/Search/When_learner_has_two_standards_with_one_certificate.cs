using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_learner_has_two_standards_with_one_certificate : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetSubmittedAndDraftCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate
                    {
                        CertificateReference = "00010001",
                        StandardCode = 12,
                        CertificateData =
                            JsonConvert.SerializeObject(new CertificateData {OverallGrade = "Distinction"})
                    }
                });

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CertificateLog>
                {
                    new CertificateLog
                    {
                        Status = CertificateStatus.Submitted,
                        Username = "username",
                        EventTime = DateTime.UtcNow

                    }
                });


            IlrRepository.Setup(r => r.SearchForLearnerByUlnAndFamilyName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr()
                    {
                        Id = Guid.NewGuid(), StdCode = 12, FamilyName = "Lamora", Uln = 1111111111,
                        CompletionStatus = CompletionStatus.Completed
                    },
                    new Ilr()
                    {
                        Id = Guid.NewGuid(), StdCode = 13, FamilyName = "Lamora", Uln = 1111111111,
                        CompletionStatus = CompletionStatus.Completed
                    }
                });

            IlrRepository.Setup(r => r.RefreshFromSubmissionEventData(It.IsAny<Guid>(), It.IsAny<SubmissionEvent>()))
                .Returns(Task.CompletedTask);
            IlrRepository.Setup(r => r.MarkAllUpToDate(It.IsAny<long>())).Returns(Task.CompletedTask);

            ContactRepository.Setup(r => r.GetContact("username")).ReturnsAsync(new Contact
            {
                OrganisationId = Guid.Parse("B79EF91B-DB69-445F-B1EC-007B8C1B5D0D")
            });
        }

        [Test]
        public void Then_a_response_is_returned_with_one_certificate_reference_and_one_none()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", UkPrn = 12345, Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;
            result.Count.Should().Be(2);
            result[0].CertificateReference.Should().Be("00010001");
            result[1].CertificateReference.Should().BeNull();
        }
    }
}