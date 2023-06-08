using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


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
        public async Task Handle_ShouldOptInSuccessfully_WhenValidRequestIsProvided()
        {
            var request = new OrganisationStandardVersionOptInRequest
            {
                ContactId = Guid.NewGuid(),
                StandardReference = "ST0001",
                Version = "1.0",
                EndPointAssessorOrganisationId = "EPA0001"
            };
            var contact = new Contact { Id = request.ContactId, Email = "Email" };
            var organisationStandard = new OrganisationStandard { Id = 101 };
            var standard = new Standard { StandardUId = "ST0001_1.0", Version = request.Version };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId)).ReturnsAsync(contact);
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference)).ReturnsAsync(organisationStandard);
            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference)).ReturnsAsync(new List<Standard> { standard });
            _organisationStandardRepositoryMock.Setup(x => x.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version)).ReturnsAsync((OrganisationStandardVersion)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(Unit.Value, result);
            _organisationStandardRepositoryMock.Verify(x => x.CreateOrganisationStandardVersion(It.IsAny<OrganisationStandardVersion>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<SendOptInStandardVersionEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // You can add more test cases here, such as what happens if an exception is thrown, or to test the different branches of your code.
    }
}


