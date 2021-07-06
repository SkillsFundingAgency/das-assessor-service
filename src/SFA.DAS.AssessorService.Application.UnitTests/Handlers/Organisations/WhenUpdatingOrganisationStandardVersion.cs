using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
    public class WhenUpdatingOrganisationStandardVersion
    {
        private Mock<IEpaOrganisationValidator> _mockValidator;
        private Mock<IOrganisationStandardRepository> _mockOrganisationStandardRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private UpdateOrganisationStandardVersionRequest _request;

        private UpdateOrganisationStandardVersionHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _mockValidator = new Mock<IEpaOrganisationValidator>();
            _mockOrganisationStandardRepository = new Mock<IOrganisationStandardRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            var fixture = new Fixture();

            _request = fixture.Build<UpdateOrganisationStandardVersionRequest>().Create();

            _handler = new UpdateOrganisationStandardVersionHandler(_mockValidator.Object, _mockOrganisationStandardRepository.Object, _mockUnitOfWork.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_UpdateOrganisationStandardVersion(OrganisationStandardVersion version)
        {
            SetupGetOrganisationStandardVersions(version);

            await _handler.Handle(_request, CancellationToken.None);

            _mockOrganisationStandardRepository.Verify(
                repository => repository.UpdateOrganisationStandardVersion(It.Is<OrganisationStandardVersion>(v => v.EffectiveFrom == _request.EffectiveFrom 
                    && v.EffectiveTo == _request.EffectiveTo)), 
                Times.Once);
        }

        private void SetupGetOrganisationStandardVersions(OrganisationStandardVersion version)
        {
            _mockOrganisationStandardRepository.Setup(repository => repository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(version);
        }
    }
}
