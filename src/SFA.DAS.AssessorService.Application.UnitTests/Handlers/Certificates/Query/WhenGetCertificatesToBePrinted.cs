using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificatesToBePrinted
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private List<CertificateResponse> _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var certificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew().With(cd => cd.OverallGrade = "Pass").Build());
            var failedCertData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew().With(cd => cd.OverallGrade = "Fail").Build());

            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(6).With(q => q.CertificateData = certificateData)
                .TheNext(1).With(q => q.CertificateData = failedCertData)
                .TheNext(3).With(q => q.CertificateData = certificateData)
                .Build().ToList();

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificates(It.IsAny<List<string>>())).Returns(Task.FromResult(certificates));

            var getCertificatesToBePrintedHandler =
                new GetCertificatesToBePrintedHandler(_certificateRepository.Object, Mock.Of<ILogger<GetCertificatesHistoryHandler>>());

            _result = getCertificatesToBePrintedHandler.Handle(new GetToBePrintedCertificatesRequest(), new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Count().Should().Be(9);
        }

        [Test]
        public void then_no_failed_certificates_are_returned()
        {
            _result.Select(r => r.CertificateData.OverallGrade).Should().NotContain("Fail");
        }
    }
}
