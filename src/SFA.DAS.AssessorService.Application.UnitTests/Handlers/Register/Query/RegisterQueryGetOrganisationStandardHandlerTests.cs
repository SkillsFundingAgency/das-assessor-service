using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    public class RegisterQueryGetOrganisationStandardHandlerTests
    {
        private Mock<IRegisterQueryRepository> _mockRegisterQueryRepository;
        private Mock<IStandardService> _mockStandardService;

        private GetOrganisationStandardHandler _handler;

        private EpaOrganisation _organisationResponse;
        private StandardCollation _standardResponse;
        private IEnumerable<OrganisationStandardVersion> _versions;

        [SetUp]
        public void Arrange()
        {
            _mockStandardService = new Mock<IStandardService>();
            _mockRegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            var fixture = new Fixture();

            var organisationStandardResponse = fixture.Build<OrganisationStandard>().Create();
            _organisationResponse = fixture.Create<EpaOrganisation>();
            _standardResponse = fixture.Create<StandardCollation>();

            _versions = fixture.Create<IEnumerable<OrganisationStandardVersion>>();

            _mockRegisterQueryRepository.Setup(r => r.GetOrganisationStandardFromOrganisationStandardId(It.IsAny<int>()))
                .ReturnsAsync(organisationStandardResponse);

            _mockRegisterQueryRepository.Setup(r => r.GetEpaOrganisationByOrganisationId(organisationStandardResponse.OrganisationId))
                .ReturnsAsync(_organisationResponse);

            _mockStandardService.Setup(ss => ss.GetStandard(organisationStandardResponse.StandardId))
                .ReturnsAsync(_standardResponse);

            _mockStandardService.Setup(ss => ss.GetEPAORegisteredStandardVersions(_organisationResponse.OrganisationId, _standardResponse.StandardId))
                .ReturnsAsync(_versions);

            _handler = new GetOrganisationStandardHandler(_mockRegisterQueryRepository.Object, Mock.Of<ILogger<GetAssessmentOrganisationHandler>>(), _mockStandardService.Object);
        }

        [Test]
        public async Task Then_CallGetRegisteredStandardVersions_And_AddToResponse()
        {
            var result = await _handler.Handle(new GetOrganisationStandardRequest(), CancellationToken.None);

            _mockStandardService.Verify(ss => ss.GetEPAORegisteredStandardVersions(_organisationResponse.OrganisationId, _standardResponse.StandardId), Times.Once());

            result.Versions.Should().BeEquivalentTo(_versions);
        }
    }
}
