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
    public class WhenHandlingGetStandardVersionsByOrganisationIdAndStandardReferenceRequest
    {
        [Test, MoqAutoData]
        public async Task ThenGetsAllStandardVersions(
            [Frozen] Mock<IRegisterQueryRepository> repository,
            GetAppliedStandardVersionsForEPAORequest request,
            List<AppliedStandardVersion> versions,
            GetAppliedStandardVersionsForEPAOHandler sut)
        {
            //Arrange
            repository.Setup(s => s.GetAppliedStandardVersionsForEPAO(request.OrganisationId, request.StandardReference)).ReturnsAsync(versions);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().BeEquivalentTo(versions);
        }
    }
}
