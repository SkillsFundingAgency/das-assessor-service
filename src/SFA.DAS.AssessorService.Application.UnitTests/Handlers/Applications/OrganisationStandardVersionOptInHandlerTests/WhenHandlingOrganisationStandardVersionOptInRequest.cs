using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OrganisationStandards
{
    public class WhenHandlingOrganisationStandardVersionOptInRequest
    {
        private OrganisationStandardVersionOptInHandler _sut;
        private Mock<IOrgansiationStandardRepository> _mockRepository;
        private Mock<IApplyRepository> _mockApplyRepository;
        private Mock<IStandardRepository> _mockStandardRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;
        private Mock<IEMailTemplateQueryRepository> _mockEMailTemplateQueryRepository;
        private Mock<IMediator> _mockMediator;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<OrganisationStandardVersionOptInHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockRepository = new Mock<IOrgansiationStandardRepository>();
            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockStandardRepository = new Mock<IStandardRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _mockEMailTemplateQueryRepository = new Mock<IEMailTemplateQueryRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<OrganisationStandardVersionOptInHandler>>();

            _sut = new OrganisationStandardVersionOptInHandler(_mockRepository.Object, _mockApplyRepository.Object, _mockStandardRepository.Object,
            _mockContactQueryRepository.Object, _mockEMailTemplateQueryRepository.Object, _mockMediator.Object,
            _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Test]
        public async Task ThenOrganisationStandardIsCreated()
        {
            var applicationId = Guid.NewGuid();

            var apply = new Domain.Entities.Apply()
            {
                Id = applicationId,
                ApplyData = new ApplyData()
                {
                    Apply = new Apply()
                }
            };

            var request = new OrganisationStandardVersionOptInRequest()
            {
                EndPointAssessorOrganisationId = "ORG",
                StandardReference = "ST0001",
                StandardUId = "ST0001_1_2",
                Version = 1.2M,
                ApplicationId = applicationId
            };

            //Arrange
            _mockRepository.Setup(m => m.GetOrganisationStandardByOrganisationIdAndStandardReference("ORG", "ST0001"))
                                    .ReturnsAsync(new Domain.Entities.OrganisationStandard() { Id = 123 });

            _mockApplyRepository.Setup(m => m.GetApply(applicationId))
                                .ReturnsAsync(apply);

            _mockContactQueryRepository.Setup(m => m.GetContactById(It.IsAny<Guid>()))
                                .ReturnsAsync(new Domain.Entities.Contact() { Email = "a@b.com", DisplayName = "Bob Smith" });

            _mockStandardRepository.Setup(m => m.GetStandardVersionByStandardUId("ST0001_1_2"))
                                .ReturnsAsync(new Domain.Entities.Standard());

            _mockEMailTemplateQueryRepository.Setup(m => m.GetEmailTemplate(EmailTemplateNames.APPLY_EPAO_RESPONSE))
                                .ReturnsAsync(new Domain.DTOs.EmailTemplateSummary());

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockRepository.Verify(m => m.CreateOrganisationStandardVersion(It.Is<Domain.Entities.OrganisationStandardVersion>(x =>
                    x.OrganisationStandardId == 123 &&
                    x.StandardUId == "ST0001_1_2" &&
                    x.Version == 1.2M)));

            _mockApplyRepository.Verify(m => m.SubmitApplicationSequence(It.Is<Domain.Entities.Apply>(x =>
                    x.Id == applicationId &&
                    x.ApplicationStatus == ApplicationStatus.Approved &&
                    x.ReviewStatus == ApplicationStatus.Approved &&
                    x.StandardReference == "ST0001")));

            _mockMediator.Verify(m => m.Send(It.Is<SendEmailRequest>(x => x.Email == "a@b.com"), It.IsAny<CancellationToken>()));
        }
    }
}
