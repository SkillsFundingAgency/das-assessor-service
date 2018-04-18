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

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificates
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private List<CertificateResponse> _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var certificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew().Build());
            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .All().With(q => q.CertificateData = certificateData).Build().ToList();

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificates(It.IsAny<string>())).Returns(Task.FromResult(certificates));

            var getCertificatesToBePrintedHandler =
                new GetCertificatesHandler(_certificateRepository.Object);

            _result = getCertificatesToBePrintedHandler.Handle(new GetCertificatesRequest(), new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Count().Should().BeGreaterOrEqualTo(10);
        }
    }
}
