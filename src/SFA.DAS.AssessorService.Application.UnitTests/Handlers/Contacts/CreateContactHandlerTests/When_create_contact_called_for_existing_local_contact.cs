using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using StackExchange.Redis;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.CreateContactHandlerTests
{
    [TestFixture]
    public class When_create_contact_called_for_non_existing_local_contact_but_existing_login_user
    {
        [SetUp]
        public void Arrange()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CreateContactRequest, Contact>().ReverseMap();
            });
        }
        
        [Test]
        public async Task Then_local_contact_is_created()
        {
            var contactRepository = new Mock<IContactRepository>();
            contactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(default(Contact));
            var newContactId = Guid.NewGuid();
            contactRepository.Setup(r => r.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact(){Id = newContactId});

            var contactQueryRepository = new Mock<IContactQueryRepository>();
            contactQueryRepository.Setup(r => r.GetAllPrivileges()).ReturnsAsync(new List<Privilege>());

            var signInService = new Mock<IDfeSignInService>();
            signInService.Setup(sis => sis.InviteUser("user@email.com", "Dave", "Smith", newContactId)).ReturnsAsync(new InviteUserResponse(){});

            var mediator = new Mock<IMediator>();

            var createContactHandler = new CreateContactHandler(contactRepository.Object, contactQueryRepository.Object, signInService.Object, mediator.Object);

            await createContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            contactRepository.Verify(r => r.CreateNewContact(It.Is<Contact>(c => c.Email == "user@email.com")));
        }
    }
}