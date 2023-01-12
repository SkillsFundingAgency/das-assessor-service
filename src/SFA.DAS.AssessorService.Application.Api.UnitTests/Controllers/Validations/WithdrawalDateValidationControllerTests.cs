using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Controllers.Validations;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Validations
{
    public class WithdrawalDateValidationControllerTests
    {
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IMediator> _mockMediator;

        private Guid _applicationId = Guid.NewGuid();
        private WithdrawalDateValidationController _controller;

        [SetUp]
        public void SetUp()
        {
            _qnaApiClient = new Mock<IQnaApiClient>();
            _mockMediator = new Mock<IMediator>();

            _mockMediator.Setup(r => r.Send<ApplicationResponse>(It.IsAny<GetApplicationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationResponse()
                {
                    ApplicationId = _applicationId
                });

            _controller = new WithdrawalDateValidationController(_qnaApiClient.Object, _mockMediator.Object, Mock.Of<ILogger<WithdrawalDateValidationController>>());
        }

        [Test]
        public async Task WhenDateIsAfterEarliestDateOfWithdrawal_ThenPasses()
        {
            // Arrange
            _qnaApiClient.Setup(r => r.GetApplicationData(_applicationId))
                .ReturnsAsync(new ApplicationData()
                {
                    PipelinesCount = 1,
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "2,6,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task WhenDateIsOnEarliestDateOfWithdrawal_ThenPasses()
        {
            // Arrange
            _qnaApiClient.Setup(r => r.GetApplicationData(_applicationId))
                .ReturnsAsync(new ApplicationData()
                {
                    PipelinesCount = 1,
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "1,6,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task WhenDateIsBeforeEarliestDateOfWithdrawal_ThenFails()
        {
            // Arrange
            _qnaApiClient.Setup(r => r.GetApplicationData(_applicationId))
                .ReturnsAsync(new ApplicationData()
                {
                    PipelinesCount = 1,
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "31,5,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeFalse();
            validationResult.ErrorMessages.Should().Contain(x => x.Value == "Date must not be before 1 Jun 2021");
        }

        [Test]
        public async Task WhenDateIsBeforeEarliestDateOfWithdrawalAndPipelinesCountIsZero_ThenPasses()
        {
            // Arrange
            _qnaApiClient.Setup(r => r.GetApplicationData(_applicationId))
                .ReturnsAsync(new ApplicationData()
                {
                    PipelinesCount = 0,
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "31,5,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task WhenDateIsInvalid_ThenFails()
        {
            // Arrange
            _qnaApiClient.Setup(r => r.GetApplicationData(_applicationId))
                .ReturnsAsync(new ApplicationData()
                {
                    PipelinesCount = 1,
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "31,5")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeFalse();
        }
    }
}
