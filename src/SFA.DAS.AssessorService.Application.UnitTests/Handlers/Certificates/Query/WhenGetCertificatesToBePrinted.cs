using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificatesToBePrinted
    {
        /*private Mock<ICertificateRepository> _certificateRepository;
        private CertificatesToBePrintedResponse _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var certificatesToBePrinted = Builder<CertificateToBePrintedSummary>.CreateListOfSize(10)
                .TheFirst(6).With(q => q.OverallGrade = CertificateGrade.Pass)
                .TheNext(1).With(q => q.OverallGrade = CertificateGrade.Fail)
                .TheNext(3).With(q => q.OverallGrade = CertificateGrade.Pass)
                .Build().ToList();

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificatesToBePrinted(It.IsAny<List<string>>())).Returns(Task.FromResult(certificatesToBePrinted));

            var getCertificatesToBePrintedHandler =
                new GetBatchCertificatesReadyToPrintHandler(_certificateRepository.Object, Mock.Of<ILogger<GetCertificatesHistoryHandler>>());

            _result = getCertificatesToBePrintedHandler.Handle(new GetBatchCertificatesReadyToPrintRequest(), new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Certificates.Count().Should().Be(9);
        }

        [Test]
        public void then_no_failed_certificates_are_returned()
        {
            _result.Certificates.Select(r => r.OverallGrade).Should().NotContain(CertificateGrade.Fail);
        }*/
    }
}
