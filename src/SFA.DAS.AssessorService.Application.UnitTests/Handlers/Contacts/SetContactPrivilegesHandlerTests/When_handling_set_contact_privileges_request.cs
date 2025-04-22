using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Handlers.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.SetContactPrivilegesHandlerTests
{
    [TestFixture]
    public class SetContactPrivilegesTestBase
    {
        protected Mock<IContactRepository> ContactRepository;
        protected Guid ContactId;
        protected Guid PrivilegeId1;
        protected Guid PrivilegeId2;
        protected Guid PrivilegeId3;
        protected Guid PrivilegeId4;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        
        protected SetContactPrivilegesHandler Handler;
        protected Guid AmendingContactId;

        [SetUp]
        public void SetUp()
        {
            PrivilegeId1 = Guid.NewGuid();
            PrivilegeId2 = Guid.NewGuid();
            PrivilegeId3 = Guid.NewGuid();
            PrivilegeId4 = Guid.NewGuid();
            
            ContactRepository = new Mock<IContactRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();

            ContactId = Guid.NewGuid();
            AmendingContactId = Guid.NewGuid();
            
            _contactQueryRepository.Setup(repo => repo.GetPrivilegesFor(ContactId)).ReturnsAsync(new List<ContactsPrivilege>()
            {
                new ContactsPrivilege {PrivilegeId = PrivilegeId1},
                new ContactsPrivilege {PrivilegeId = PrivilegeId2},
                new ContactsPrivilege {PrivilegeId = PrivilegeId3, Privilege = new Privilege(){Id = PrivilegeId3, MustBeAtLeastOneUserAssigned = true}}
            });

            _contactQueryRepository.Setup(repo => repo.GetAllPrivileges()).ReturnsAsync(new List<Privilege>()
            {
                new Privilege {Id = PrivilegeId1},
                new Privilege {Id = PrivilegeId2},
                new Privilege {Id = PrivilegeId3},
                new Privilege {Id = PrivilegeId4},
            });

            _contactQueryRepository.Setup(repo => repo.GetContactById(ContactId)).ReturnsAsync(new Contact() {Email = "email@address.com"});
            _contactQueryRepository.Setup(repo => repo.GetContactById(AmendingContactId)).ReturnsAsync(new Contact() {Email = "amender@address.com"});
            
            Handler = new SetContactPrivilegesHandler(ContactRepository.Object, _contactQueryRepository.Object, new Mock<IMediator>().Object);
        }
    }
    
    public class When_handling_set_contact_privileges_request : SetContactPrivilegesTestBase
    {
        [SetUp]
        public async Task Act()
        {
            await Handler.Handle(new SetContactPrivilegesRequest{AmendingContactId = AmendingContactId, ContactId = ContactId, PrivilegeIds = new []{PrivilegeId1, PrivilegeId2}}, CancellationToken.None);
        }
        
        [Test]
        public void Then_existing_privileges_are_deleted()
        {
            ContactRepository.Verify(repo => repo.RemoveAllPrivileges(ContactId));
        }
        
        [Test]
        public void Then_new_privileges_are_created()
        {
            ContactRepository.Verify(repo => repo.AddPrivilege(ContactId, PrivilegeId1));
            ContactRepository.Verify(repo => repo.AddPrivilege(ContactId, PrivilegeId2));
        }

        [Test]
        public void Then_check_is_carried_out_that_contact_isnt_last_contact_removing_user_management()
        {
            ContactRepository.Verify(repo => repo.IsOnlyContactWithPrivilege(ContactId, PrivilegeId3));
        }

        [Test]
        public void Then_email_is_sent_with_correct_tokens()
        {
            
        }
    }
}