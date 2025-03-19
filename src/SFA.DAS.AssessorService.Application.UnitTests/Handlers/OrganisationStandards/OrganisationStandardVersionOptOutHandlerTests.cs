using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;


namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OrganisationStandards
{
    [TestFixture]
    public class OrganisationStandardVersionOptOutHandlerTests
    {
        private OrganisationStandardVersionOptOutHandler _handler;
        private Mock<IOrganisationStandardRepository> _organisationStandardRepositoryMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<OrganisationStandardVersionOptOutHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _organisationStandardRepositoryMock = new Mock<IOrganisationStandardRepository>();
            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrganisationStandardVersionOptOutHandler>>();

            _handler = new OrganisationStandardVersionOptOutHandler(_organisationStandardRepositoryMock.Object, _contactQueryRepositoryMock.Object,
                _mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldOptOutSuccessfully_WhenValidRequestIsProvidedForStandardVersionWhichDoesExist()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptOutRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = DateTime.Today,
                ContactId = Guid.NewGuid(),
                OptOutRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version };
            var organisationStandardVersion = new OrganisationStandardVersion { StandardUId = "ST0001_1.0", Comments = $"Opted in by EPAO {contact.Email} at {request.OptOutRequestedAt.AddDays(-10)}" };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandard);

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version))
                .ReturnsAsync(organisationStandardVersion);

            var entity = new AssessorService.Api.Types.Models.AO.OrganisationStandardVersion
            {
                StandardUId = standard.StandardUId,
                Version = standard.Version,
                OrganisationStandardId = organisationStandard.Id,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateVersionApproved = request.OptOutRequestedAt,
                Comments = $"{organisationStandardVersion.Comments};Opted out by EPAO {contact.Email} at {request.OptOutRequestedAt}",
                Status = OrganisationStatus.Live
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            entity.Should().BeEquivalentTo(result);
            _organisationStandardRepositoryMock.Verify(x => x.UpdateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<SendOptOutStandardVersionEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_ContactId_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptOutRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptOutRequestedAt = DateTime.Now
            };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(It.IsAny<Guid>()))
                .ReturnsAsync((Domain.Entities.Contact)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out to StandardReference {request.StandardReference} as ContactId {request.ContactId} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_OrganisationStandard_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptOutRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptOutRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            
            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync((OrganisationStandard)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_OptOutVersion_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptOutRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptOutRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandard);
            
            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} for {request.EndPointAssessorOrganisationId} cannot be found");
        }
    }
}


