using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards
{
    public class WhenHandlingGetStandardByOrganisationAndReferenceRequest
    {
        [Test, MoqAutoData]
        public async Task ThenGetsLatestStandardVersion(
            [Frozen] Mock<IRegisterQueryRepository> repository,
            GetStandardByOrganisationAndReferenceRequest request,
            OrganisationStandard standard,
            List<OrganisationStandardVersion> versions,
            GetStandardByOrganisationAndReferenceHandler sut)
        {
            //Arrange
            repository.Setup(s => s.GetOrganisationStandardFromOrganisationIdAndStandardRefence(request.OrganisationId, request.StandardReference)).ReturnsAsync(standard);
            repository.Setup(s => s.GetOrganisationStandardVersionsByOrganisationStandardId(standard.Id)).ReturnsAsync(versions);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Versions.Should().BeEquivalentTo(versions);
        }
    }
}
