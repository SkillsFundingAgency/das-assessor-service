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
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificatesToBePrinted
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private List<Certificate> _result;

        [SetUp]
        public void Arrange()
        {
            var certificates = Builder<Certificate>.CreateListOfSize(10).Build().ToList();

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificatesToBePrinted()).Returns(Task.FromResult(certificates));

            var getCertificatesToBePrintedHandler =
                new GetCertificatesToBePrintedHandler(_certificateRepository.Object);

            _result = getCertificatesToBePrintedHandler.Handle(new GetCertificatesToBePrintedRequest(), new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Count().Should().BeGreaterOrEqualTo(10);
        }
    }
}
