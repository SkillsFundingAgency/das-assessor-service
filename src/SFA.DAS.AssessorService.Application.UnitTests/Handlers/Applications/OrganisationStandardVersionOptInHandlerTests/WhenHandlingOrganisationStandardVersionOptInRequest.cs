using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
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
        private Mock<IOrganisationStandardRepository> _mockRepository;
        private Mock<IApplyRepository> _mockApplyRepository;
        private Mock<IStandardRepository> _mockStandardRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;
        private Mock<IEMailTemplateQueryRepository> _mockEMailTemplateQueryRepository;
        private Mock<IMediator> _mockMediator;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<OrganisationStandardVersionOptInHandler>> _mockLogger;

        private Guid  _applicationId;
        private Domain.DTOs.EmailTemplateSummary _emailTemplate;
        private List<string> _emailRequestTokens;

        [SetUp]
        public void Arrange()
        {
            _mockRepository = new Mock<IOrganisationStandardRepository>();
            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockStandardRepository = new Mock<IStandardRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _mockEMailTemplateQueryRepository = new Mock<IEMailTemplateQueryRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<OrganisationStandardVersionOptInHandler>>();

            _applicationId = Guid.NewGuid();
            _emailRequestTokens = new List<string>();
            _emailTemplate = new Domain.DTOs.EmailTemplateSummary();

            _mockRepository.Setup(m => m.GetOrganisationStandardByOrganisationIdAndStandardReference("ORG", "ST0001"))
                                    .ReturnsAsync(new Domain.Entities.OrganisationStandard() { Id = 123 });

            _mockApplyRepository.Setup(m => m.GetApply(_applicationId))
                                .ReturnsAsync(new Domain.Entities.Apply()
                                {
                                    Id = _applicationId,
                                    ApplyData = new ApplyData()
                                    {
                                        Apply = new Apply()
                                    }
                                });

            _mockContactQueryRepository.Setup(m => m.GetContactById(It.IsAny<Guid>()))
                                .ReturnsAsync(new Domain.Entities.Contact() { Email = "a@b.com", DisplayName = "Bob Smith" });

            _mockStandardRepository.Setup(m => m.GetStandardVersionByStandardUId("ST0001_1_2"))
                                .ReturnsAsync(new Domain.Entities.Standard()
                                {
                                    Title = "TITLE"
                                });

            _mockEMailTemplateQueryRepository.Setup(m => m.GetEmailTemplate(EmailTemplateNames.ApplyEPAOStandardOptin))
                                .ReturnsAsync(_emailTemplate);

            _mockMediator
                .Setup(c =>
                    c.Send(
                        It.IsAny<SendEmailRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<IRequest, CancellationToken>((request, token) =>
                {
                    var sendEmailRequest = request as SendEmailRequest;
                    _emailRequestTokens.Add(JsonConvert.SerializeObject(sendEmailRequest.Tokens));
                });

            _sut = new OrganisationStandardVersionOptInHandler(_mockRepository.Object, _mockApplyRepository.Object, _mockStandardRepository.Object,
            _mockContactQueryRepository.Object, _mockEMailTemplateQueryRepository.Object, _mockMediator.Object,
            _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Test]
        public async Task ThenOrganisationStandardIsCreated()
        {
            //Arrange
            var request = new OrganisationStandardVersionOptInRequest()
            {
                EndPointAssessorOrganisationId = "ORG",
                StandardReference = "ST0001",
                StandardUId = "ST0001_1_2",
                Version = 1.2M,
                ApplicationId = _applicationId
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
         }

        [Test]
        public async Task ThenApplicationIsSubmitted()
        {
            //Arrange
            var request = new OrganisationStandardVersionOptInRequest()
            {
                EndPointAssessorOrganisationId = "ORG",
                StandardReference = "ST0001",
                StandardUId = "ST0001_1_2",
                Version = 1.2M,
                ApplicationId = _applicationId
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockApplyRepository.Verify(m => m.SubmitApplicationSequence(It.Is<Domain.Entities.Apply>(x =>
                    x.Id == _applicationId &&
                    x.ApplicationStatus == ApplicationStatus.Approved &&
                    x.ReviewStatus == ApplicationStatus.Approved &&
                    x.StandardReference == "ST0001")));
        }

        [Test]
        public async Task ThenEmailIsSent()
        {
            //Arrange
            var request = new OrganisationStandardVersionOptInRequest()
            {
                EndPointAssessorOrganisationId = "ORG",
                StandardReference = "ST0001",
                StandardUId = "ST0001_1_2",
                Version = 1.2M,
                ApplicationId = _applicationId
            };

            var expectedTokens = JsonConvert.SerializeObject(new
            {
                contactname = "Bob Smith",
                standard = "TITLE",
                version = "1.2",
            });

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockMediator.Verify(m => m.Send(It.Is<SendEmailRequest>(x => x.Email == "a@b.com" && x.EmailTemplateSummary == _emailTemplate), 
                                                It.IsAny<CancellationToken>()));

            _emailRequestTokens.Should().Contain(expectedTokens);
        }

        [Test]
        public async Task AndSomethingFailsThenEmailIsNotSent()
        {
            //Arrange
            var request = new OrganisationStandardVersionOptInRequest()
            {
                EndPointAssessorOrganisationId = "ORG",
                StandardReference = "ST0001",
                StandardUId = "ST0001_1_3",
                Version = 1.2M,
                ApplicationId = _applicationId
            };

            _mockRepository.Setup(m => m.CreateOrganisationStandardVersion(It.IsAny< Domain.Entities.OrganisationStandardVersion>()))
                            .Throws(new Exception());

            //Act
            try
            {
                await _sut.Handle(request, new CancellationToken());
            }
            catch(Exception)
            { }

            //Assert
            _mockMediator.Verify(m => m.Send(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
