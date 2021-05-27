using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.GetBatchLearner
{
    public class WhenHandlingGetBatchLearnerHandler
    {
        private GetBatchLearnerRequest _request;

        private Mock<IMediator> _mockMediator;
        private Mock<IIlrRepository> _mockIlrRepository;
        private Mock<IOrganisationQueryRepository> _mockOrgQueryRepository;
        private Mock<IStandardService> _mockStandardService;
        private Mock<ICertificateRepository> _mockCertificateRepoistory;

        private CertificateData _certificateData;
        private Standard _standardResponse;
        private Ilr _ilrResponse;
        private Organisation _epaoResponse;
        private Certificate _certificateResponse;

        private GetBatchLearnerHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _request = Builder<GetBatchLearnerRequest>.CreateNew()
                .With(r => r.IncludeCertificate = true).Build();

            _certificateData = Builder<CertificateData>.CreateNew().Build();

            _ilrResponse = Builder<Ilr>.CreateNew().Build();
            _epaoResponse = Builder<Organisation>.CreateNew().Build();
            _certificateResponse = Builder<Certificate>.CreateNew()
                .With(cr => cr.CertificateData = JsonConvert.SerializeObject(_certificateData))
                .Build();
            _standardResponse = Builder<Standard>.CreateNew().Build();

            _mockMediator = new Mock<IMediator>();
            _mockIlrRepository = new Mock<IIlrRepository>();
            _mockOrgQueryRepository = new Mock<IOrganisationQueryRepository>();
            _mockStandardService = new Mock<IStandardService>();
            _mockCertificateRepoistory = new Mock<ICertificateRepository>();

            _mockStandardService.Setup(ss => ss.GetStandardVersionById(_request.Standard, null))
                .ReturnsAsync(_standardResponse);

            _mockIlrRepository.Setup(ilr => ilr.Get(_request.Uln, It.IsAny<int>()))
                .ReturnsAsync(_ilrResponse);

            _mockOrgQueryRepository.Setup(org => org.GetByUkPrn(It.IsAny<long>()))
                .ReturnsAsync(_epaoResponse);

            _mockMediator.Setup(m => m.Send(It.IsAny<GetBatchCertificateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_certificateResponse);

            _mockCertificateRepoistory.Setup(cr => cr.GetCertificate(_request.Uln, It.IsAny<int>(), _request.FamilyName))
                .ReturnsAsync(_certificateResponse);

            _handler = new GetBatchLearnerHandler(_mockMediator.Object, 
                Mock.Of<ILogger<GetBatchLearnerHandler>>(),
                _mockIlrRepository.Object, 
                _mockOrgQueryRepository.Object, 
                _mockStandardService.Object,
                _mockCertificateRepoistory.Object);
        }

        [Test]
        public async Task Then_ReturnLearnerResponse()
        {
            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Certificate.Should().BeEquivalentTo(_certificateResponse);
            result.EpaDetails.Should().BeEquivalentTo(_certificateData.EpaDetails);
        }

        [Test]
        public async Task Then_StandardServiceIsCalled()
        {
            await _handler.Handle(_request, CancellationToken.None);

            _mockStandardService.Verify(s => s.GetStandardVersionById(_request.Standard, null), Times.Once);
        }

        [Test]
        public async Task And_StandardNotFound_Then_ReturnNull()
        {
            _mockStandardService.Setup(ss => ss.GetStandardVersionById(_request.Standard, null))
                .ReturnsAsync((Standard)null);

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeOfType<GetBatchLearnerResponse>();
            result.Certificate.Should().BeNull();
            result.Learner.Should().BeNull();
        }

        [Test]
        public async Task And_IlrNotFound_Then_ReturnNull()
        {
            _mockIlrRepository.Setup(ilr => ilr.Get(_request.Uln, It.IsAny<int>()))
                .ReturnsAsync((Ilr)null);

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeOfType<GetBatchLearnerResponse>();
            result.Learner.Should().BeNull();
        }

    }
}
