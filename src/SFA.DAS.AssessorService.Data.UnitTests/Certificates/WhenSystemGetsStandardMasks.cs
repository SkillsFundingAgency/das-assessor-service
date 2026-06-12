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
    public class WhenSystemGetsStandardMasks
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
                new CertificateMask { CertificateType = "Standard", CourseCode = "100", CourseName = "Std Test", CourseLevel = "3", ProviderName = "Prov" }
            };

            _mockUnitOfWork.Setup(u => u.QueryStoredProcedureAsync<CertificateMask>("Certificates_GetStandardMasks", It.IsAny<DynamicParameters>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResults);

            var result = await _certificateRepository.GetStandardMasks(new long[] { 1111111111 });

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().CourseCode.Should().Be("100");

            _mockUnitOfWork.Verify(u => u.QueryStoredProcedureAsync<CertificateMask>("Certificates_GetStandardMasks", It.IsAny<DynamicParameters>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
