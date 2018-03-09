using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Post.Handlers
{
    public class WhenOrganisationHasExistingContractsCreateContactHandlerSucceeds
    { 
        private ContactResponse _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var organisationRepositoryMock = new Mock<IOrganisationRepository>();
            var organisation = Builder<OrganisationDomainModel>.CreateNew().Build();
            var organisationQueryRepositoryMock = CreateOrganisationQueryRepositoryMock(organisation);

            var contactResponse = Builder<ContactResponse>.CreateNew().Build();
            var createContactRequest = Builder<CreateContactRequest>.CreateNew().Build();
            var contactRepositoryMock = CreateContactRepositoryMock(contactResponse);

            var createContactHandler = new CreateContactHandler(organisationRepositoryMock.Object, organisationQueryRepositoryMock.Object, contactRepositoryMock.Object);

            _result = createContactHandler.Handle(createContactRequest, new CancellationToken()).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = _result as ContactResponse;
            result.Should().NotBeNull();
        }

        private static Mock<IContactRepository> CreateContactRepositoryMock(ContactResponse contactResponse)
        {
            var contactRepositoryMock = new Mock<IContactRepository>();
            contactRepositoryMock.Setup(q => q.CreateNewContact(Moq.It.IsAny<CreateContactDomainModel>()))
                .Returns(Task.FromResult((contactResponse)));
            contactRepositoryMock.Setup(q => q.CreateNewContact(Moq.It.IsAny<CreateContactDomainModel>()))
                .Returns(Task.FromResult(contactResponse));
            return contactRepositoryMock;
        }

        private static Mock<IOrganisationQueryRepository> CreateOrganisationQueryRepositoryMock(OrganisationDomainModel organisation)
        {
            var organisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();
            organisationQueryRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(organisation));
            organisationQueryRepositoryMock.Setup(q => q.CheckIfOrganisationHasContacts(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));
            return organisationQueryRepositoryMock;
        }
    }
}

