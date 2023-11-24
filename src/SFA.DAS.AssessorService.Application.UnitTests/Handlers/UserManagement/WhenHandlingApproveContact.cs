using AutoFixture.NUnit3;
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

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.UserManagement
{
    public class WhenHandlingApproveContact
    {
        [Test, MoqAutoData]
        public async Task Then_ApproveConfirmation_Email_Sent_For_Gov_Login_Enabled(
            [Frozen] Mock<IContactQueryRepository> contactQueryRepository,
            [Frozen] Mock<IContactRepository> contactRepository,
            [Frozen] Mock<IOrganisationQueryRepository> organisationQueryRepository,
            [Frozen] Mock<IEmailApiClient> emailApiClient,
            [Frozen] Mock<IApiConfiguration> apiConfiguration,
            EmailTemplatesConfig emailTemplatesConfig,
            ApproveContactRequest request,
            ApproveContactHandler sut)
        {
            //Arrange
            var contact = new Contact { Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };

            contactQueryRepository.Setup(s => s.GetContactById(request.ContactId)).ReturnsAsync(contact);
            organisationQueryRepository.Setup(s => s.Get(contact.OrganisationId.Value)).ReturnsAsync(new Organisation());

            contactRepository.Setup(x => x.UpdateContactWithOrganisationData(It.IsAny<UpdateContactWithOrgAndStausRequest>())).ReturnsAsync(contact);
            emailApiClient.Setup(x => x.SendEmailWithTemplate(It.IsAny<SendEmailRequest>()))
                .Returns(Task.CompletedTask);
            
            apiConfiguration.Setup(x => x.EmailTemplatesConfig).Returns(emailTemplatesConfig);
            apiConfiguration.Setup(x => x.ServiceLink).Returns(It.IsAny<string>());
            apiConfiguration.Setup(x => x.UseGovSignIn).Returns(true);

            //Act
            await sut.Handle(request, new CancellationToken());

            //Assert
            emailApiClient.Verify(x => x.SendEmailWithTemplate(It.IsAny<SendEmailRequest>()), Times.Once);
        }
        
        [Test, MoqAutoData]
        public async Task Then_ApproveConfirmation_Email_Not_Sent_For_Gov_Login_Disabled(
            [Frozen] Mock<IContactQueryRepository> contactQueryRepository,
            [Frozen] Mock<IContactRepository> contactRepository,
            [Frozen] Mock<IOrganisationQueryRepository> organisationQueryRepository,
            [Frozen] Mock<IEmailApiClient> emailApiClient,
            [Frozen] Mock<IApiConfiguration> apiConfiguration,
            EmailTemplatesConfig emailTemplatesConfig,
            ApproveContactRequest request,
            ApproveContactHandler sut)
        {
            //Arrange
            var contact = new Contact { Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };

            contactQueryRepository.Setup(s => s.GetContactById(request.ContactId)).ReturnsAsync(contact);
            organisationQueryRepository.Setup(s => s.Get(contact.OrganisationId.Value)).ReturnsAsync(new Organisation());

            contactRepository.Setup(x => x.UpdateContactWithOrganisationData(It.IsAny<UpdateContactWithOrgAndStausRequest>())).ReturnsAsync(contact);
            emailApiClient.Setup(x => x.SendEmailWithTemplate(It.IsAny<SendEmailRequest>()))
                .Returns(Task.CompletedTask);
            
            apiConfiguration.Setup(x => x.EmailTemplatesConfig).Returns(emailTemplatesConfig);
            apiConfiguration.Setup(x => x.ServiceLink).Returns(It.IsAny<string>());
            apiConfiguration.Setup(x => x.UseGovSignIn).Returns(false);

            //Act
            await sut.Handle(request, new CancellationToken());

            //Assert
            emailApiClient.Verify(x => x.SendEmailWithTemplate(It.IsAny<SendEmailRequest>()), Times.Never);
        }
    }
}

