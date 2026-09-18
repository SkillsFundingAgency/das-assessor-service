using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;


namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OrganisationStandards
{
    [TestFixture]
    public class OrganisationStandardVersionOptInHandlerTests
    {
        private OrganisationStandardVersionOptInHandler _handler;
        private Mock<IOrganisationStandardRepository> _organisationStandardRepositoryMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<IStandardService> _standardServiceMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<OrganisationStandardVersionOptInHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _organisationStandardRepositoryMock = new Mock<IOrganisationStandardRepository>();
            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _standardServiceMock = new Mock<IStandardService>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrganisationStandardVersionOptInHandler>>();

            _handler = new OrganisationStandardVersionOptInHandler(_organisationStandardRepositoryMock.Object, _contactQueryRepositoryMock.Object,
                _mediatorMock.Object, _standardServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldOptInSuccessfully_WhenValidRequestIsProvidedForStandardVersionWhichDoesNotExist()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 456 };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard> { organisationStandard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version))
                .ReturnsAsync((OrganisationStandardVersion)null);

            var entity = new AssessorService.Api.Types.Models.AO.OrganisationStandardVersion
            {
                StandardUId = standard.StandardUId,
                Version = standard.Version,
                OrganisationStandardId = organisationStandard.Id,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateVersionApproved = request.OptInRequestedAt,
                Comments = $"Opted in by EPAO {contact.Email} at {request.OptInRequestedAt}",
                Status = OrganisationStatus.Live
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            entity.Should().BeEquivalentTo(result);
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.Is<OrganisationStandardVersion>(
                version => version.OrganisationStandardId == organisationStandard.Id)), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<SendOptInStandardVersionEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldOptInSuccessfully_WhenValidRequestIsProvidedForStandardVersionWhichDoesExist()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = standard.LarsCode };
            var organisationStandardVersion = new OrganisationStandardVersion { Comments = $"Opted in by EPAO {contact.Email} at {request.OptInRequestedAt.AddDays(-100)}" };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard> { organisationStandard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version))
                .ReturnsAsync(organisationStandardVersion);

            var entity = new AssessorService.Api.Types.Models.AO.OrganisationStandardVersion
            {
                StandardUId = standard.StandardUId,
                Version = standard.Version,
                OrganisationStandardId = organisationStandard.Id,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateVersionApproved = request.OptInRequestedAt,
                Comments = $"{organisationStandardVersion.Comments};Opted in by EPAO {contact.Email} at {request.OptInRequestedAt}",
                Status = OrganisationStatus.Live
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            entity.Should().BeEquivalentTo(result);
            _organisationStandardRepositoryMock.Verify(x => x.UpdateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<SendOptInStandardVersionEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldOptInSuccessfully_WhenMultipleOrganisationStandardsExistAndOneMatchesStandardCode()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };
            var nonMatchingOrganisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 456 };
            var matchingOrganisationStandard = new OrganisationStandard { Id = 102, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = standard.LarsCode };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard>
                {
                    nonMatchingOrganisationStandard,
                    matchingOrganisationStandard
                });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(matchingOrganisationStandard.Id, request.Version))
                .ReturnsAsync((OrganisationStandardVersion)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.OrganisationStandardId.Should().Be(matchingOrganisationStandard.Id);
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.Is<OrganisationStandardVersion>(
                version => version.OrganisationStandardId == matchingOrganisationStandard.Id)), Times.Once);
            _organisationStandardRepositoryMock.Verify(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(
                nonMatchingOrganisationStandard.Id, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_ContactId_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(It.IsAny<Guid>()))
                .ReturnsAsync((Domain.Entities.Contact)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt in to StandardReference {request.StandardReference} as ContactId {request.ContactId} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_OrganisationStandard_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard>());

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt in as StandardReference {request.StandardReference} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_OptInVersion_Is_Not_Found()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard>());

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt in as StandardReference {request.StandardReference} Version {request.Version} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_BadRequestException_When_MultipleOrganisationStandardsExistAndNoneMatchStandardCode()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard>
                {
                    new OrganisationStandard { Id = 101, StandardCode = 456 },
                    new OrganisationStandard { Id = 102, StandardCode = 789 }
                });

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<BadRequestException>()
                .WithMessage($"Cannot uniquely identify OrganisationStandard for {request.EndPointAssessorOrganisationId}, {request.StandardReference} and StandardCode {standard.LarsCode}");

            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Never);
            _organisationStandardRepositoryMock.Verify(x => x.UpdateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Throw_BadRequestException_When_MultipleOrganisationStandardsMatchStandardCode()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0001",
                Version = "1.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = DateTime.Now
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 123 };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .ReturnsAsync(new List<Standard> { standard });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(new List<OrganisationStandard>
                {
                    new OrganisationStandard { Id = 101, StandardCode = standard.LarsCode },
                    new OrganisationStandard { Id = 102, StandardCode = standard.LarsCode }
                });

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<BadRequestException>()
                .WithMessage($"Cannot uniquely identify OrganisationStandard for {request.EndPointAssessorOrganisationId}, {request.StandardReference} and StandardCode {standard.LarsCode}");

            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Never);
            _organisationStandardRepositoryMock.Verify(x => x.UpdateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Never);
        }
    }
}