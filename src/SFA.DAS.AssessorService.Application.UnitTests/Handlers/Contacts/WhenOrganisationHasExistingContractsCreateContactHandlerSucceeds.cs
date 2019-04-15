﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OfficeOpenXml.ConditionalFormatting;
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
            
            var dfeSignInServiceMock = new Mock<IDfeSignInService>();
            var contactResponse = Builder<Contact>.CreateNew().Build();
            var contactRequest = Builder<CreateContactRequest>
                .CreateNew().Build();
            var contactRepositoryMock = CreateContactRepositoryMock(contactResponse);
            var mediator = new Mock<IMediator>();
            var contactQueryRepository = new Mock<IContactQueryRepository>();

            dfeSignInServiceMock.Setup(x =>
                    x.InviteUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new InviteUserResponse{IsSuccess=true}));
            var createContactHandler = new CreateContactHandler( contactRepositoryMock.Object, contactQueryRepository.Object,
                dfeSignInServiceMock.Object, mediator.Object, new Mock<ILogger<CreateContactHandler>>().Object);

            _result = createContactHandler.Handle(contactRequest, new CancellationToken()).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = _result;
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

