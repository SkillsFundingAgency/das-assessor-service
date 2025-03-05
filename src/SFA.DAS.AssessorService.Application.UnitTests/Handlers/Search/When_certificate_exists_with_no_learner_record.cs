using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_certificate_exists_with_no_learner_record : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner>());
        }

        [Test]
        public void Then_search_result_is_returned()
        {
            SetUpCertificateAndLogEntries(12);

            var result = SearchHandler.Handle(new LearnerSearchRequest { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" }, CancellationToken.None).Result;

            result.Count.Should().Be(1); 
        }

        [Test]
        public void and_learnername_does_not_match_then_search_result_is_not_returned()
        {
            SetUpCertificateAndLogEntries(12);

            var result = SearchHandler.Handle(new LearnerSearchRequest { Surname = "UnknownName", EpaOrgId = "12345", Uln = 1111111111, Username = "username" }, CancellationToken.None).Result;

            result.Count.Should().Be(0);
        }

        [Test]
        public void and_standard_is_not_approved_then_search_result_is_not_returned()
        {
            SetUpCertificateAndLogEntries(27);

            var result = SearchHandler.Handle(new LearnerSearchRequest { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" }, CancellationToken.None).Result;

            result.Count.Should().Be(0);
        }

        private void SetUpCertificateAndLogEntries(int standardCode)
        {
            var certificateData = Builder<CertificateData>.CreateNew()
                .With(x => x.LearnerFamilyName = "Lamora").Build();

            var certificate = Builder<Certificate>.CreateNew()
                .With(x => x.StandardCode = standardCode)
                .With(x => x.CertificateData = JsonConvert.SerializeObject(certificateData))
                .Build();

            var certificatesResponse = new List<Certificate> { certificate };

            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(certificatesResponse);

            var certificateLogEntries = Builder<CertificateLog>.CreateListOfSize(2)
                .All()
                    .With(x => x.CertificateId = certificate.Id)
                .TheFirst(1)
                    .With(x => x.Status == CertificateStatus.Draft)
                    .With(x => x.EventTime == DateTime.Now.AddDays(-1))
                .TheNext(1)
                    .With(x => x.Status == CertificateStatus.Submitted)
                    .With(x => x.EventTime == DateTime.Now)
                .Build().ToList();

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(certificate.Id))
                .ReturnsAsync(certificateLogEntries);
        }
    }
}
