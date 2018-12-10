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
    public class When_search_completed : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateId = Guid.NewGuid();
            var searchingEpaoOrgId = Guid.NewGuid();

            CertificateRepository.Setup(r => r.GetSubmittedAndDraftCertificatesFor(1111111111))
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
                        CreatedBy = "username"
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
            IlrRepository.Setup(r => r.RefreshFromSubmissionEventData(It.IsAny<Guid>(), It.IsAny<SubmissionEvent>()))
                .Returns(Task.CompletedTask);
            IlrRepository.Setup(r => r.MarkAllUpToDate(It.IsAny<long>())).Returns(Task.CompletedTask);
            ContactRepository.Setup(cr => cr.GetContact("username"))
                .ReturnsAsync(new Contact() {DisplayName = "EPAO User from this EAPOrg", OrganisationId = searchingEpaoOrgId});


            IlrRepository.Setup(r => r.SearchForLearnerByUlnAndFamilyName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr()
                    {
                        Id = Guid.NewGuid(), StdCode = 12, FamilyName = "Lamora", Uln = 1111111111,
                        CompletionStatus = CompletionStatus.Completed
                    }
                });
        }                                                           

        [Test]
        public void Then_a_response_is_returned_including_LearnStartDate()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", UkPrn = 12345, Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;

            result[0].LearnStartDate.Should().Be(new DateTime(2015, 06, 01));
        }

        [Test]
        public void Then_a_Seach_Log_entry_is_created()
        {
            SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", UkPrn = 12345, Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Wait();

            IlrRepository.Verify(r => r.StoreSearchLog(It.Is<SearchLog>(l =>
                l.Username == "username" && l.NumberOfResults == 1 && l.Surname == "Lamora" && l.Uln == 1111111111)));
        }
    }
}