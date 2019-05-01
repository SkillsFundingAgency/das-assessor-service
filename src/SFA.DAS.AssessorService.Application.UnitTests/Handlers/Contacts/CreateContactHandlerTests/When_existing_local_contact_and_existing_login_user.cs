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
    public class When_existing_local_contact_and_existing_login_user : CreateContactsHandlerTestBase
    {
        private Guid _existingLoginUserId;
        private Guid _existingContactUserId;

        [SetUp]
        public void Arrange()
        {
            _existingLoginUserId = Guid.NewGuid();
            _existingContactUserId = Guid.NewGuid();
            
            ContactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(new Contact(){Id = _existingContactUserId});
            ContactQueryRepository.Setup(r => r.GetAllPrivileges()).ReturnsAsync(new List<Privilege>());
            
            SignInService.Setup(sis => sis.InviteUser("user@email.com", "Dave", "Smith", _existingContactUserId))
                .ReturnsAsync(new InviteUserResponse(){UserExists = true, ExistingUserId = _existingLoginUserId, IsSuccess = false});
        }

        [Test]
        public async Task Then_local_contact_is_not_created()
        {
            await CreateContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            ContactRepository.Verify(r => r.CreateNewContact(It.Is<Contact>(c => c.Email == "user@email.com")), Times.Never);
        }

        [Test]
        public async Task Then_local_contact_is_updated_with_signInId()
        {
            await CreateContactHandler.Handle(new CreateContactRequest{FamilyName = "Smith", Email = "user@email.com", GivenName = "Dave"}, CancellationToken.None);
            
            ContactRepository.Verify(r => r.UpdateSignInId(_existingContactUserId, _existingLoginUserId));
        }
    }
}