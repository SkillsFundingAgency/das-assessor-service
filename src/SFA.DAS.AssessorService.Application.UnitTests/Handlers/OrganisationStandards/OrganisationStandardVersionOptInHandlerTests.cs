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
    public class OrganisationStandardVersionOptInHandlerTests
    {
        private OrganisationStandardVersionOptInHandler _handler;
        private Mock<IOrganisationStandardRepository> _organisationStandardRepositoryMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<IStandardService> _standardServiceMock;
        private Mock<IOrganisationStandardSelector> _organisationStandardSelectorMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<OrganisationStandardVersionOptInHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _organisationStandardRepositoryMock = new Mock<IOrganisationStandardRepository>();
            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _standardServiceMock = new Mock<IStandardService>();
            _organisationStandardSelectorMock = new Mock<IOrganisationStandardSelector>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<OrganisationStandardVersionOptInHandler>>();

            _handler = new OrganisationStandardVersionOptInHandler(_organisationStandardRepositoryMock.Object, _contactQueryRepositoryMock.Object,
                _mediatorMock.Object, _standardServiceMock.Object, _organisationStandardSelectorMock.Object, _loggerMock.Object);
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
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Once);
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
            var organisationStandard = new OrganisationStandard { Id = 101, EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId, StandardCode = 12345 };
            var organisationStandards = new List<OrganisationStandard> { organisationStandard };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };
            var organisationStandardVersion = new OrganisationStandardVersion { Comments = $"Opted in by EPAO {contact.Email} at { request.OptInRequestedAt.AddDays(-100) }" };

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
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 12345 };
            var organisationStandards = new List<OrganisationStandard>();
            var notFoundMessage = $"Cannot opt in as StandardReference {request.StandardReference} LarsCode {standard.LarsCode} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found (0 existing OrganisationStandard records)";

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
                .WithMessage(notFoundMessage);
        }

        [Test]
        public async Task Handle_ShouldCreateNewOrganisationStandard_WhenSelectionRequiresNewRecord()
        {
            // Arrange
            var request = new OrganisationStandardVersionOptInRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardReference = "ST0490",
                Version = "2.0",
                EffectiveFrom = DateTime.Now.AddDays(-100),
                EffectiveTo = null,
                ContactId = Guid.NewGuid(),
                OptInRequestedAt = new DateTime(2026, 7, 15, 15, 32, 9)
            };

            var contact = new Contact { Id = request.ContactId, Email = "emailaddress@test.com" };
            var standard = new Standard { StandardUId = $"{request.StandardReference}_{request.Version}", Version = request.Version, LarsCode = 99999 };
            var template = new OrganisationStandard
            {
                Id = 101,
                EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId,
                StandardCode = 12345,
                StandardReference = request.StandardReference,
                DateStandardApprovedOnRegister = new DateTime(2020, 1, 1),
                ContactId = Guid.NewGuid()
            };
            var organisationStandards = new List<OrganisationStandard> { template };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId))
                .ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync(standard);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference))
                .ReturnsAsync(organisationStandards);
            _organisationStandardSelectorMock.Setup(x => x.Select(organisationStandards, standard.LarsCode))
                .Returns(new OrganisationStandardSelectionOutcome { Result = OrganisationStandardSelectionResult.RequiresNewRecord, TemplateOrganisationStandard = template, TotalRowCount = 1, MatchingLarsCodeCount = 0 });

            const int newOrganisationStandardId = 201;
            _organisationStandardRepositoryMock.Setup(x => x.CreateOrganisationStandard(It.IsAny<OrganisationStandard>()))
                .Callback<OrganisationStandard>(os => os.Id = newOrganisationStandardId)
                .ReturnsAsync(newOrganisationStandardId);

            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(newOrganisationStandardId, request.Version))
                .ReturnsAsync((OrganisationStandardVersion)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandard(It.Is<OrganisationStandard>(os =>
                os.EndPointAssessorOrganisationId == request.EndPointAssessorOrganisationId &&
                os.StandardCode == standard.LarsCode &&
                os.EffectiveFrom == request.OptInRequestedAt &&
                os.EffectiveTo == null &&
                os.DateStandardApprovedOnRegister == template.DateStandardApprovedOnRegister &&
                os.Comments == $"Added by EPAO {contact.Email} at 15/07/2026 15:32:09" &&
                os.Status == OrganisationStatus.Live &&
                os.ContactId == template.ContactId &&
                os.OrganisationStandardData == null &&
                os.StandardReference == request.StandardReference)), Times.Once);

            result.OrganisationStandardId.Should().Be(newOrganisationStandardId);
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.Is<OrganisationStandardVersion>(v => v.OrganisationStandardId == newOrganisationStandardId)), Times.Once);
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
            _standardServiceMock.Setup(x => x.GetStandardVersionById(request.StandardReference, request.Version))
                .ReturnsAsync((Standard)null);

            // Act
            Func<Task<AssessorService.Api.Types.Models.AO.OrganisationStandardVersion>> func = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Cannot opt in as StandardReference {request.StandardReference} Version {request.Version} cannot be found");

            _organisationStandardRepositoryMock.Verify(x => x.GetOrganisationStandardsByOrganisationIdAndStandardReference(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}


