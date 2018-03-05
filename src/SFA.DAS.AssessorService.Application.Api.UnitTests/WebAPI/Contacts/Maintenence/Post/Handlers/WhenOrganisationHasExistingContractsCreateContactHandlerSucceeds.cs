﻿using System.Threading;
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
        private Contact _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var organisationRepositoryMock = new Mock<IOrganisationRepository>();
            var organisation = Builder<OrganisationQueryDomainModel>.CreateNew().Build();
            var organisationQueryRepositoryMock = CreateOrganisationQueryRepositoryMock(organisation);

            var contactQueryViewModel = Builder<Contact>.CreateNew().Build();
            var createContactRequest = Builder<CreateContactRequest>.CreateNew().Build();
            var contactRepositoryMock = CreateContactRepositoryMock(contactQueryViewModel);

            var createContactHandler = new CreateContactHandler(organisationRepositoryMock.Object, organisationQueryRepositoryMock.Object, contactRepositoryMock.Object);

            _result = createContactHandler.Handle(createContactRequest, new CancellationToken()).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = _result as Contact;
            result.Should().NotBeNull();
        }

        private static Mock<IContactRepository> CreateContactRepositoryMock(Contact contactQueryViewModel)
        {
            var contactRepositoryMock = new Mock<IContactRepository>();
            contactRepositoryMock.Setup(q => q.CreateNewContact(Moq.It.IsAny<ContactCreateDomainModel>()))
                .Returns(Task.FromResult((contactQueryViewModel)));
            contactRepositoryMock.Setup(q => q.CreateNewContact(Moq.It.IsAny<ContactCreateDomainModel>()))
                .Returns(Task.FromResult(contactQueryViewModel));
            return contactRepositoryMock;
        }

        private static Mock<IOrganisationQueryRepository> CreateOrganisationQueryRepositoryMock(OrganisationQueryDomainModel organisation)
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

