using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
    public class WhenGettingOrganisationByUkprn
    {
        [Test]
        public async Task Then_The_Repository_Is_Called_And_OrganisationResponse_Is_Returned()
        {
            //Arrange
            var fixture = new Fixture();
            long ukprn = fixture.Create<long>();
            var organisation = fixture
                .Build<Organisation>()
                .Without(o => o.Certificates)
                .Without(o => o.Contacts)
                .Without(o => o.OrganisationStandards)
                .Create();

            var mockRepo = new Mock<IOrganisationQueryRepository>();
            mockRepo.Setup(r => r.GetByUkPrn(ukprn)).ReturnsAsync(organisation);

            AutoMapper.Mapper.Reset();
            Mapper.Initialize(cfg => cfg.AddProfile<AssessorServiceOrganisationResponse>());

            var sut = new GetOrganisationByUkprnHandler(mockRepo.Object);

            //Act
            var result = await sut.Handle(new GetOrganisationByUkprnRequest(ukprn), new CancellationToken());

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<OrganisationResponse>(result);
            
            Assert.AreEqual(organisation.Id, result.Id);
            Assert.AreEqual(organisation.PrimaryContact, result.PrimaryContact);
            Assert.AreEqual(organisation.Status, result.Status);
            Assert.AreEqual(organisation.EndPointAssessorName, result.EndPointAssessorName);
            Assert.AreEqual(organisation.EndPointAssessorOrganisationId, result.EndPointAssessorOrganisationId);
            Assert.AreEqual(organisation.EndPointAssessorUkprn, result.EndPointAssessorUkprn);
            Assert.AreEqual(organisation.OrganisationData.RoATPApproved, result.RoATPApproved);
            Assert.AreEqual(organisation.OrganisationData.RoEPAOApproved, result.RoEPAOApproved);
            Assert.AreEqual(organisation.OrganisationType.Type, result.OrganisationType);
            Assert.AreEqual(organisation.OrganisationData.CompanySummary, result.CompanySummary);
            Assert.AreEqual(organisation.OrganisationData.CharitySummary, result.CharitySummary);
        }
    }
}