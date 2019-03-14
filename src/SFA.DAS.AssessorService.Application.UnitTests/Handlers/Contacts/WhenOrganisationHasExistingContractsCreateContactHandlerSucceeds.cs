using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts
{
    public class WhenOrganisationHasExistingContractsCreateContactHandlerSucceeds
    { 
        private ContactBoolResponse _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var organisationRepositoryMock = new Mock<IOrganisationRepository>();
            var organisation = Builder<Organisation>.CreateNew().Build();
            var organisationQueryRepositoryMock = CreateOrganisationQueryRepositoryMock(organisation);

            var contactResponse = Builder<Contact>.CreateNew().Build();
            var createContactRequest = Builder<CreateContactRequest>.CreateNew().Build();
            var contactRepositoryMock = CreateContactRepositoryMock(contactResponse);
            var mediator = new Mock<IMediator>();
            var defSignInService = new Mock<IDfeSignInService>();

            var createContactHandler = new CreateContactHandler(organisationRepositoryMock.Object, organisationQueryRepositoryMock.Object, contactRepositoryMock.Object, defSignInService.Object,mediator.Object);

            _result = createContactHandler.Handle(createContactRequest, new CancellationToken()).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = _result as ContactBoolResponse;
            result.Result.Should().BeTrue();
        }

        private static Mock<IContactRepository> CreateContactRepositoryMock(Contact contactResponse)
        {
            var contactRepositoryMock = new Mock<IContactRepository>();
            contactRepositoryMock.Setup(q => q.CreateNewContact(It.IsAny<Contact>()))
                .Returns(Task.FromResult((contactResponse)));
            contactRepositoryMock.Setup(q => q.CreateNewContact(It.IsAny<Contact>()))
                .Returns(Task.FromResult(contactResponse));
            return contactRepositoryMock;
        }

        private static Mock<IOrganisationQueryRepository> CreateOrganisationQueryRepositoryMock(Organisation organisation)
        {
            var organisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();
            organisationQueryRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(organisation));
            organisationQueryRepositoryMock.Setup(q => q.CheckIfOrganisationHasContacts(It.IsAny<string>()))
                .Returns(Task.FromResult((true)));
            return organisationQueryRepositoryMock;
        }
    }
}

