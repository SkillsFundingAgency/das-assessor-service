using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_learner_has_two_standards_with_one_certificate : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate {CertificateReference = "00010001", StandardCode = 12,
                        CertificateData =
                            JsonConvert.SerializeObject(new CertificateData {})}
                });

            CertificateRepository.Setup(r => r.GetCertificateLogsFor(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CertificateLog>());

            IlrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr> {new Ilr() {StdCode = 12, FamilyName = "Lamora"}, new Ilr() {StdCode = 13, FamilyName = "Lamora"}});
        }

        [Test]
        public void Then_a_response_is_returned_with_one_certificate_reference_and_one_none()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;
            result.Count.Should().Be(2);
            result[0].CertificateReference.Should().Be("00010001");
            result[1].CertificateReference.Should().BeNull();
        }
    }
}