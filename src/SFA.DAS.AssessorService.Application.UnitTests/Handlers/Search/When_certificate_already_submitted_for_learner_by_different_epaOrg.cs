using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

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

            CertificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
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
                .ReturnsAsync(new List<CertificateLog>(){new CertificateLog(){Status = CertificateStatus.Submitted, EventTime = new DateTime(2018,2,3,13,23,33), Username = "differentuser"}});

            ContactRepository.Setup(cr => cr.GetContact("differentuser"))
                .ReturnsAsync(new Contact() {DisplayName = "EPAO User from another EAPOrg", OrganisationId = submittingEpaoOrgId});

            ContactRepository.Setup(cr => cr.GetContact("username"))
                .ReturnsAsync(new Contact() {DisplayName = "EPAO User from this EAPOrg", OrganisationId = searchingEpaoOrgId});


            IlrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr> {new Ilr() {StdCode = 12, FamilyName = "Lamora"}});
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