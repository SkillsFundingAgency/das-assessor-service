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
        private Mock<IContactRepository> _contactRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<IDfeSignInService> _signInService;
        private Mock<IMediator> _mediator;
        private CreateContactHandler _createContactHandler;

        [SetUp]
        public void Arrange()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CreateContactRequest, Contact>().ReverseMap();
            });
            
            _contactRepository = new Mock<IContactRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _signInService = new Mock<IDfeSignInService>();
            _mediator = new Mock<IMediator>();
            _createContactHandler = new CreateContactHandler(_contactRepository.Object, _contactQueryRepository.Object, _signInService.Object, _mediator.Object);
        }
        
        [Test]
        public async Task Then_local_contact_is_created()
        {
            _contactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(default(Contact));
            var newContactId = Guid.NewGuid();
            _contactRepository.Setup(r => r.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact(){Id = newContactId});
            _contactQueryRepository.Setup(r => r.GetAllPrivileges()).ReturnsAsync(new List<Privilege>());
            _signInService.Setup(sis => sis.InviteUser("user@email.com", "Dave", "Smith", newContactId)).ReturnsAsync(new InviteUserResponse(){});

            await _createContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            _contactRepository.Verify(r => r.CreateNewContact(It.Is<Contact>(c => c.Email == "user@email.com")));
        }
        
        [Test]
        public async Task Then_local_contact_is_updated_with_signInId()
        {
            _contactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(default(Contact));
            var newContactId = Guid.NewGuid();
            _contactRepository.Setup(r => r.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact(){Id = newContactId});
            _contactQueryRepository.Setup(r => r.GetAllPrivileges()).ReturnsAsync(new List<Privilege>());
            var existingUserId = Guid.NewGuid();
            _signInService.Setup(sis => sis.InviteUser("user@email.com", "Dave", "Smith", newContactId)).ReturnsAsync(new InviteUserResponse(){UserExists = true, ExistingUserId = existingUserId, IsSuccess = false});

            await _createContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            _contactRepository.Verify(r => r.UpdateSignInId(newContactId, existingUserId));
        }
    }
}