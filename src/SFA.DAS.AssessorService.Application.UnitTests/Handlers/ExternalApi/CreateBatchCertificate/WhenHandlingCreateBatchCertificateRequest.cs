using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.CreateBatchCertificate
{
    public class WhenHandlingCreateBatchCertificateRequest
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IIlrRepository> _ilrRepository;
        private Mock<IOrganisationQueryRepository> _organisationQueryRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<ILogger<CreateBatchCertificateHandler>> _logger;
        private Mock<IStandardService> _standardService;
        private Mock<IRoatpApiClient> _roatpApiClient;

        private CreateBatchCertificateHandler _handler;

        private long uln = 12345678L;
        private int stdCode = 123;
        private int ukPrn = 111;
        private string stdUId = "ST0123_1.0";
        private CreateBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _ilrRepository = new Mock<IIlrRepository>();
            _organisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<CreateBatchCertificateHandler>>();
            _standardService = new Mock<IStandardService>();
            _roatpApiClient = new Mock<IRoatpApiClient>();

            _ilrRepository.Setup(m => m.Get(uln, stdCode)).ReturnsAsync(new Domain.Entities.Ilr() {});
            
            _organisationQueryRepository.Setup(m => m.GetByUkPrn(ukPrn)).ReturnsAsync(new Organisation() { });
            
            _standardService.Setup(m => m.GetStandardVersionById(stdUId, null)).ReturnsAsync(new Standard() { });
            _standardService.Setup(m => m.GetStandardOptionsByStandardId(stdUId)).ReturnsAsync(new StandardOptions());

            _roatpApiClient.Setup(m => m.GetOrganisationByUkprn(ukPrn)).ReturnsAsync(new OrganisationSearchResult()
            {
                ProviderName = "PROVIDER"
            });

            _request = new CreateBatchCertificateRequest()
            {
                StandardCode = stdCode,
                StandardUId = stdUId,
                Uln = uln,
                UkPrn = ukPrn,
                CertificateData = new Domain.JsonData.CertificateData()
            };

            _handler = new CreateBatchCertificateHandler(_certificateRepository.Object, _ilrRepository.Object, _organisationQueryRepository.Object,
                _contactQueryRepository.Object, _logger.Object, _standardService.Object, _roatpApiClient.Object);
        }

        [Test]
        public async Task AndCertificateAlreadyExistsThenReturnsCertificate()
        {
            // Arrange
            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode)).ReturnsAsync(new Certificate()
            {
                CertificateData = @"{}"
            });

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);


            //Assert
            _certificateRepository.Verify(m => m.Update(It.IsAny<Certificate>(), ExternalApiConstants.ApiUserName, CertificateActions.Start, true, null));

            result.StandardUId.Should().Equals(stdUId);
            result.Status.Should().Equals(CertificateStatus.Draft);
        }

        [Test]
        public async Task AndCertificateDoesNotExistThenReturnsCertificate()
        {
            // Arrange
            var id = Guid.NewGuid();

            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode)).ReturnsAsync((Certificate)null);

            _certificateRepository.Setup(m => m.New(It.Is<Certificate>(c => c.Uln == uln &&
                       c.StandardCode == stdCode &&
                       c.CreatedBy == ExternalApiConstants.ApiUserName &&
                       c.Status == CertificateStatus.Draft)))
                .ReturnsAsync(new Certificate() { Id = id });

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            _certificateRepository.Verify(m => m.Update(It.IsAny<Certificate>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Never);

            result.Id.Should().Equals(id);
        }
    }
}