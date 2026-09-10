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
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
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
        private Mock<IStandardService> _standardServiceMock;
        private Mock<IOrganisationStandardSelector> _organisationStandardSelectorMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<OrganisationStandardVersionOptOutHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _organisationStandardRepositoryMock = new Mock<IOrganisationStandardRepository>();
            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _standardServiceMock = new Mock<IStandardService>();
            _organisationStandardSelectorMock = new Mock<IOrganisationStandardSelector>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrganisationStandardVersionOptOutHandler>>();

            _handler = new OrganisationStandardVersionOptOutHandler(_organisationStandardRepositoryMock.Object, _contactQueryRepositoryMock.Object,
                _mediatorMock.Object, _standardServiceMock.Object, _organisationStandardSelectorMock.Object, _loggerMock.Object);
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
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 12345 };
            var organisationStandards = new List<OrganisationStandard> { organisationStandard };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };
            var organisationStandardVersion = new OrganisationStandardVersion { StandardUId = "ST0001_1.0", Comments = $"Opted in by EPAO {contact.Email} at {request.OptOutRequestedAt.AddDays(-10)}" };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);

            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync(standard);

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandards);
            _organisationStandardSelectorMock.Setup(x => x.Select(organisationStandards, standard.LarsCode))
                .Returns(new OrganisationStandardSelectionOutcome { Result = OrganisationStandardSelectionResult.Found, OrganisationStandard = organisationStandard, TotalRowCount = 1, MatchingLarsCodeCount = 0 });

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version))
                .ReturnsAsync(organisationStandardVersion);

            var entity = new AssessorService.Api.Types.Models.AO.OrganisationStandardVersion
            {
                StandardUId = organisationStandardVersion.StandardUId,
                Version = request.Version,
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
        public async Task Handle_Should_Throw_NotFoundException_When_StandardVersion_Is_Not_Found()
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
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync((Standard)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} cannot be found");

            // Should fail before ever needing to resolve the OrganisationStandard row
            _organisationStandardRepositoryMock.Verify(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };
            var organisationStandards = new List<OrganisationStandard>();

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync(standard);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandards);
            _organisationStandardSelectorMock.Setup(x => x.Select(organisationStandards, standard.LarsCode))
                .Returns(new OrganisationStandardSelectionOutcome { Result = OrganisationStandardSelectionResult.NotFound, TotalRowCount = 0, MatchingLarsCodeCount = 0 });

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_OrganisationStandard_Is_Ambiguous()
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
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };
            var organisationStandards = new List<OrganisationStandard>
            {
                new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 99999 },
                new OrganisationStandard { Id = 102, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 88888 }
            };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync(standard);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandards);
            _organisationStandardSelectorMock.Setup(x => x.Select(organisationStandards, standard.LarsCode))
                .Returns(new OrganisationStandardSelectionOutcome { Result = OrganisationStandardSelectionResult.Ambiguous, TotalRowCount = 2, MatchingLarsCodeCount = 0 });

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} LarsCode {standard.LarsCode} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} is ambiguous (2 records found, 0 matched the LarsCode)");
        }

        [Test]
        public async Task Handle_Should_Throw_NotFoundException_When_ExistingOrganisationStandardVersion_Is_Not_Found()
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
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 12345 };
            var organisationStandards = new List<OrganisationStandard> { organisationStandard };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync(standard);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandards);
            _organisationStandardSelectorMock.Setup(x => x.Select(organisationStandards, standard.LarsCode))
                .Returns(new OrganisationStandardSelectionOutcome { Result = OrganisationStandardSelectionResult.Found, OrganisationStandard = organisationStandard, TotalRowCount = 1, MatchingLarsCodeCount = 0 });
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version))
                .ReturnsAsync((OrganisationStandardVersion)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} for {request.EndPointAssessorOrganisationId} cannot be found");
        }
    }
}
