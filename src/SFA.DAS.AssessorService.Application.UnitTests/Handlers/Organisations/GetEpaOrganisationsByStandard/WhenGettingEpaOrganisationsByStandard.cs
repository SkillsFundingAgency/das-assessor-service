using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations.GetEpaOrganisationsByStandard
{
    public class WhenGettingEpaOrganisationsByStandard
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Repositroy_Is_Called_And_Data_Returned_In_Response(
            GetEpaOrganisationsByStandardQuery query,
            Organisation entityOrganisation,
            [Frozen] Mock<IOrganisationQueryRepository> repository,
            GetEpaOrganisationsByStandardQueryHandler handler)
        {
            //Arrange
            var entityOrganisations = new List<Organisation> {entityOrganisation};
            repository.Setup(x => x.GetOrganisationsByStandard(query.Standard)).ReturnsAsync(entityOrganisations);
            
            
            //Act
            var actual = await handler.Handle(query, It.IsAny<CancellationToken>());
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<GetEpaOrganisationsByStandardResponse>(actual);
            actual.EpaOrganisations.Should().BeEquivalentTo(entityOrganisations);
        }

    }
}
