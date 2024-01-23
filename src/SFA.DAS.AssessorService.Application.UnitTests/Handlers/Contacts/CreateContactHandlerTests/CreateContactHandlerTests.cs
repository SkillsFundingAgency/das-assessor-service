using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.CreateContactHandlerTests;

public class CreateContactHandlerTests: CreateContactsHandlerTestBase
{
    private Guid _existingContactUserId;
        
    [SetUp]
    public void Arrange()
    {
        _existingContactUserId = Guid.NewGuid();
        ContactRepository.Setup(r => r.GetContact("user@email.com")).ReturnsAsync(new Contact(){Id = _existingContactUserId});
        
    }

    [Test]
    public async Task When_Gov_Login_Identifier_Supplied_And_User_Does_Exist_Then_User_Is_Not_Invited_And_Contact_Updated_With_Guid_And_GovIdentifier()
    {
        await CreateContactHandler.Handle(
            new CreateContactRequest("Given Name", "Family Name", "user@email.com", null, null, "some-identifier"),
            CancellationToken.None);

        SignInService.Verify(
            x => x.InviteUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Never);
        ContactRepository.Verify(r => r.UpdateSignInId(_existingContactUserId, It.IsAny<Guid?>(), "some-identifier"));
    }
    
    [Test]
    public async Task When_Gov_Login_Identifier_Supplied_And_User_Doesnt_Exist_Then_User_Is_Not_Invited_And_Contact_Updated_With_Guid_And_GovIdentifier()
    {
        var contactId = Guid.NewGuid();
        ContactRepository.Setup(x => x.CreateNewContact(It.Is<Contact>(c =>
            c.SignInType.Equals("GovLogin") && c.GovUkIdentifier.Equals("some-identifier") &&
            c.Email.Equals("user2@email.com")))).ReturnsAsync(new Contact
        {
            Id = contactId
        });
        
        await CreateContactHandler.Handle(
            new CreateContactRequest("Given Name", "Family Name", "user2@email.com", null, null, "some-identifier"),
            CancellationToken.None);

        SignInService.Verify(
            x => x.InviteUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Never);
        ContactRepository.Verify(r => r.UpdateSignInId(contactId, It.IsAny<Guid?>(), "some-identifier"));
    }
}