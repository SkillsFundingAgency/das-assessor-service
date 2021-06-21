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
    public class When_certificate_exists_with_no_ilr_record : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            IlrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr>());
        }

        [Test]
        public void Then_search_result_is_returned()
        {
            SetUpCertificateAndLogEntries();

            var result = SearchHandler.Handle(new SearchQuery { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" }, CancellationToken.None).Result;

            result.Count.Should().Be(1); 
        }

        private void SetUpCertificateAndLogEntries()
        {
            var certificateData = Builder<CertificateData>.CreateNew()
                .With(x => x.LearnerFamilyName = "Lamora").Build();

            var certificate = Builder<Certificate>.CreateNew()
                .With(x => x.CertificateData = JsonConvert.SerializeObject(certificateData))
                .Build();

            var certificatesResponse = new List<Certificate> { certificate };

            CertificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
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
