using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Controllers.Validations;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Validations
{
    public class WithdrawalDateValidationControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IQnaApiClient> _mockQnaApiClient;

        private WithdrawalDateValidationController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();

            _mockMediator.Setup(r => r.Send<ApplicationResponse>(It.IsAny<GetApplicationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationResponse());
            
            _mockQnaApiClient.Setup(r => r.GetApplicationData(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationData()
                {
                    EarliestDateOfWithdrawal = new DateTime(2021, 6, 1, 10, 30, 12)
                });

            _controller = new WithdrawalDateValidationController(_mockQnaApiClient.Object, _mockMediator.Object, Mock.Of<ILogger<WithdrawalDateValidationController>>());
        }

        [Test]
        public async Task WhenDateIsAfterEarliestDateOfWithdrawal_ThenPasses()
        {
            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "2,6,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task WhenDateIsOnEarliestDateOfWithdrawal_ThenPasses()
        {
            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "1,6,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task WhenDateIsBeforeEarliestDateOfWithdrawal_ThenFails()
        {
            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "31,5,2021")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeFalse();
            validationResult.ErrorMessages.Should().Contain(x => x.Value == "Date must not be before 1 Jun 2021");
        }

        [Test]
        public async Task WhenDateIsInvalid_ThenFails()
        {
            // Act
            var result = (await _controller.ValidateWithdrawalDate(Guid.NewGuid(), "31,5")) as ActionResult<ApiValidationResult>;

            var validationResult = result.Value;
            validationResult.IsValid.Should().BeFalse();
        }
    }
}
