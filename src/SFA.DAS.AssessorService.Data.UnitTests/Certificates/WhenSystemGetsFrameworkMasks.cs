using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsFrameworkMasks
    {
        private CertificateRepository _certificateRepository;
        private Mock<IAssessorUnitOfWork> _mockUnitOfWork;

        [SetUp]
        public void Arrange()
        {
            _mockUnitOfWork = new Mock<IAssessorUnitOfWork>();

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task Then_Repository_Queries_StoredProcedure_And_Returns_Masks()
        {
            var repoResults = new List<CertificateMask>
            {
                new CertificateMask { CertificateType = "Framework", CourseCode = "F100", CourseName = "FW Test", CourseLevel = "2", ProviderName = "ProvF" }
            };

            _mockUnitOfWork.Setup(u => u.QueryStoredProcedureAsync<CertificateMask>("Certificates_GetFrameworkMasks", It.IsAny<DynamicParameters>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResults);

            var result = await _certificateRepository.GetFrameworkMasks(new long[] { 2222222222 });

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().CourseCode.Should().Be("F100");

            _mockUnitOfWork.Verify(u => u.QueryStoredProcedureAsync<CertificateMask>("Certificates_GetFrameworkMasks", It.IsAny<DynamicParameters>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
