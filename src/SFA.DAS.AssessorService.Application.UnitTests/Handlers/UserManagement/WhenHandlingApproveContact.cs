﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Handlers.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.UserManagement
{
    public class WhenHandlingApproveContact
    {
        [Test, MoqAutoData]
        public async Task Then_ApproveConfirmation_Email_Sent(
            string email,
            [Frozen] Mock<IContactQueryRepository> contactQueryRepository,
            [Frozen] Mock<IContactRepository> contactRepository,
            [Frozen] Mock<IOrganisationQueryRepository> organisationQueryRepository,
            [Frozen] Mock<IApiConfiguration> apiConfiguration,
            [Frozen] Mock<IMediator> mediator,
            EmailTemplatesConfig emailTemplatesConfig,
            ApproveContactRequest request,
            ApproveContactHandler sut)
        {
            //Arrange
            var contact = new Contact { Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid(), Email = email};

            contactQueryRepository.Setup(s => s.GetContactById(request.ContactId)).ReturnsAsync(contact);
            organisationQueryRepository.Setup(s => s.Get(contact.OrganisationId.Value)).ReturnsAsync(new Organisation());
            contactRepository.Setup(x => x.UpdateContactWithOrganisationData(It.IsAny<UpdateContactWithOrgAndStausRequest>())).ReturnsAsync(contact);
            
            apiConfiguration.Setup(x => x.EmailTemplatesConfig).Returns(emailTemplatesConfig);
            apiConfiguration.Setup(x => x.ServiceLink).Returns(It.IsAny<string>());

            //Act
            await sut.Handle(request, new CancellationToken());

            //Assert
            mediator.Verify(x => x.Send(It.Is<SendEmailRequest>(c=>c.Email.Equals(contact.Email)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

