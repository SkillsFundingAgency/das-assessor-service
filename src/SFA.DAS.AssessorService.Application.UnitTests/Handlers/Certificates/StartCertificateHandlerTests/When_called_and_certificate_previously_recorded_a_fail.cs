using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    [TestFixture]
    public class When_called_and_certificate_previously_recorded_a_fail
    {
        private Mock<ICertificateRepository> _certificateRepository;

        private StartCertificateHandler _startCertificateHandler;

        private StartCertificateRequest _request;

        private const int _standardCode = 1;
        private const long _uln = 1000000000;

        [SetUp]
        public void Arrange()
        {
            var certificateData = new Builder().CreateNew<CertificateData>()
                .With(cd => cd.OverallGrade = CertificateGrade.Fail)
                .Build();

            var certDataString = JsonConvert.SerializeObject(certificateData);

            var certificate = new Builder().CreateNew<Certificate>()
                .With(c => c.Uln = _uln)
                .With(c => c.StandardCode = _standardCode)
                .With(c => c.Status = CertificateStatus.Submitted)
                .With(c => c.CertificateData = certDataString)
                .Build();

            _certificateRepository = new Mock<ICertificateRepository>();

            _certificateRepository.Setup(r => r.GetCertificate(_uln, _standardCode))
                .ReturnsAsync(certificate);

            _request = new StartCertificateRequest
            {
                Uln = _uln,
                StandardCode = _standardCode
            };

            _startCertificateHandler = new StartCertificateHandler(_certificateRepository.Object,
                Mock.Of<IIlrRepository>(), 
                Mock.Of<IRoatpApiClient>(),
                Mock.Of<IOrganisationQueryRepository>(), 
                Mock.Of<ILogger<StartCertificateHandler>>(), 
                Mock.Of<IStandardService>());
        }

        [Test]
        public async Task Then_set_status_to_draft_and_call_update_certificate()
        {
            await _startCertificateHandler.Handle(_request, new CancellationToken());

            _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(c => c.Status == CertificateStatus.Draft && c.Uln == _uln && c.StandardCode == _standardCode),
                It.IsAny<string>(), CertificateActions.Restart, true, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Then_certificate_is_return_with_status_set_to_draft()
        {
            var certificate = await _startCertificateHandler.Handle(_request, new CancellationToken());

            certificate.Status.Should().Be(CertificateStatus.Draft);
        }

        [Test]
        public async Task Then_certificate_grade_data_is_reset()
        {
            var certificate = await _startCertificateHandler.Handle(_request, new CancellationToken());

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            certData.OverallGrade.Should().BeNull();
            certData.AchievementDate.Should().BeNull();
        }
    }
}
