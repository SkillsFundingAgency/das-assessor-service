using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    [TestFixture]
    public class GetStandardCertificateMasksHandlerTests
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IApiConfiguration> _config;
        private GetStandardCertificateMasksHandler _handler;

        [SetUp]
        public void Setup()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _config = new Mock<IApiConfiguration>();
            _config.Setup(c => c.MasksMaxCount).Returns(5);
            _handler = new GetStandardCertificateMasksHandler(_certificateRepository.Object, _config.Object);
        }

        [Test]
        public async Task Handle_ReturnsResultsFromRepository()
        {
            var repoResults = new List<CertificateMask>
            {
                new CertificateMask { CertificateType = "Standard", CourseCode = "100", CourseName = "Test Standard", CourseLevel = "3", ProviderName = "Prov" }
            };

            _certificateRepository.Setup(r => r.GetStandardMasks(It.IsAny<IEnumerable<long>>(), It.IsAny<int>()))
                .ReturnsAsync(repoResults);

            var request = new GetStandardCertificateMasksRequest { Exclude = new long[] { } };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Masks);
            Assert.AreEqual(1, result.Masks.Count);
            Assert.AreEqual("100", result.Masks[0].CourseCode);
            Assert.AreEqual("Test Standard", result.Masks[0].CourseName);
        }
    }
}
