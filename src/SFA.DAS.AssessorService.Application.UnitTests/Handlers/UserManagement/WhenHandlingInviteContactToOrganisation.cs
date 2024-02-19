using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Handlers.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.UserManagement
{
    public class WhenHandlingInviteContactToOrganisation
    {
        [Test, MoqAutoData]
        public async Task Then_Returns_ErrorResponse_WhenContactAlreadyExist(
            [Frozen] Mock<IContactQueryRepository> contactQueryRepository,
            [Frozen] Mock<IEmailApiClient> emailApiClient,
            InviteContactToOrganisationRequest request,
            InviteContactToOrganisationHandler sut)
        {
            //Arrange
            var contact = new Contact { Id = Guid.NewGuid() };
            contactQueryRepository.Setup(s => s.GetContactFromEmailAddress(request.Email)).ReturnsAsync(contact);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.ErrorMessage.Should().NotBeEmpty();
            result.Success.Should().BeFalse();

            emailApiClient.Verify(x => x.SendEmailWithTemplate(It.IsAny<SendEmailRequest>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_InviteContactToOrganisationResponse(
            string email,
            [Frozen] Mock<IContactQueryRepository> contactQueryRepository,
            [Frozen] Mock<IContactRepository> contactRepository,
            [Frozen] Mock<IOrganisationQueryRepository> organisationQueryRepository,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IApiConfiguration> apiConfiguration,
            EmailTemplatesConfig emailTemplatesConfig,
            InviteContactToOrganisationRequest request,
            InviteContactToOrganisationHandler sut)
        {
            //Arrange
            var contact = new Contact { Id = Guid.NewGuid(), Email = email};
            contactQueryRepository.Setup(s => s.GetContactFromEmailAddress(request.Email)).ReturnsAsync((Contact)null);
            organisationQueryRepository.Setup(s => s.Get(request.OrganisationId)).ReturnsAsync(new Organisation());
            contactRepository.Setup(x => x.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(contact);
            apiConfiguration.Setup(x => x.EmailTemplatesConfig).Returns(emailTemplatesConfig);
            apiConfiguration.Setup(x => x.ServiceLink).Returns(It.IsAny<string>());

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.ContactId.Should().Be(contact.Id);
            result.Success.Should().BeTrue();

            mediator.Verify(x => x.Send(It.Is<SendEmailRequest>(c=>c.Email.Equals(request.Email)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}