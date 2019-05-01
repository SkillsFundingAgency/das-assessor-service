using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.CreateContactHandlerTests
{
    public class When_non_existing_local_contact_and_non_existing_login_user : CreateContactsHandlerTestBase
    {
        private Guid _existingLoginUserId;
        private Guid _newContactId;

        [SetUp]
        public void Arrange()
        {
            _existingLoginUserId = Guid.NewGuid();
            _newContactId = Guid.NewGuid();
            
            ContactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(default(Contact));
            ContactRepository.Setup(r => r.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact(){Id = _newContactId});
            ContactQueryRepository.Setup(r => r.GetAllPrivileges()).ReturnsAsync(new List<Privilege>());
            SignInService.Setup(sis => sis.InviteUser("user@email.com", "Dave", "Smith", _newContactId))
                .ReturnsAsync(new InviteUserResponse(){UserExists = false, IsSuccess = true});
        }
        
        [Test]
        public async Task Then_local_contact_is_created()
        {
            await CreateContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            ContactRepository.Verify(r => r.CreateNewContact(It.Is<Contact>(c => c.Email == "user@email.com")));
        }
        
        [Test]
        public async Task Then_local_contact_is_not_updated_with_signInId()
        {
            await CreateContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            ContactRepository.Verify(r => r.UpdateSignInId(_newContactId, _existingLoginUserId), Times.Never);
        }
    }
}