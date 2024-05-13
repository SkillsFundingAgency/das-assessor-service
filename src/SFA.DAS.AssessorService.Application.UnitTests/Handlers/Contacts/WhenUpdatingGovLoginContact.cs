using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts;

public class WhenUpdatingGovLoginContact
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Repository_Is_Called(
        Contact contact,
        UpdateContactGovLoginRequest request,
        [Frozen] Mock<IContactRepository> contactRepository,
        UpdateContactGovLoginRequestHandler handler)
    {
        contactRepository.Setup(x => x.UpdateSignInId(request.ContactId, request.GovIdentifier))
            .ReturnsAsync(contact);
            
        var actual = await handler.Handle(request, CancellationToken.None);

        actual.Contact.Should().BeEquivalentTo(contact);
    }
}