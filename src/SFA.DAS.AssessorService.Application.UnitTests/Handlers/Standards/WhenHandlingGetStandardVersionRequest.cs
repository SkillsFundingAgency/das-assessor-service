using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards
{
    public class WhenHandlingGetStandardVersionRequest
    {
        [Test, MoqAutoData]
        public async Task ThenGetsLatestStandardVersion(
            [Frozen] Mock<IStandardService> standardService, 
            GetStandardVersionRequest request, 
            Standard standard,
            GetStandardVersionHandler sut)
        {
            //Arrange
            standardService.Setup(s => s.GetStandardVersionById(request.StandardId, request.Version)).ReturnsAsync(standard);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().BeEquivalentTo(standard);
        }
    }
}
