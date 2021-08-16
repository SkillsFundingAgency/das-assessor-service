using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Organisations
{
    [TestFixture]
    public class WhenHandlingWithdrawOrganisationRequest
    {
        private Mock<IOrganisationStandardRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private WithdrawOrganisationHandler _handler;
        private Mock<IEpaOrganisationValidator> _validator;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IOrganisationStandardRepository>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new WithdrawOrganisationHandler( _validator.Object, _repository.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task ThenWithdrawOrganisationRepoIsCalled()
        {
            // arrange 
            var withdrawDate = new DateTime(2021, 12, 1);

            var request = new WithdrawOrganisationRequest()
            {
                EndPointAssessorOrganisationId = "EPA0123",
                WithdrawalDate = withdrawDate
            };

            _validator.Setup(m => m.ValidatorWithdrawOrganisationRequest(request)).Returns(new ValidationResponse());

            var result = await _handler.Handle(request, new CancellationToken());

            _repository.Verify(r => r.WithdrawalOrganisation("EPA0123", withdrawDate));
            _unitOfWork.Verify(r => r.Commit());
        }

        [Test]
        public void AndValidationErrorThenWithdrawOrganisationRepoIsNotCalled()
        {
            // arrange 
            var request = new WithdrawOrganisationRequest();

            _validator.Setup(m => m.ValidatorWithdrawOrganisationRequest(request))
                .Returns(new ValidationResponse()
                {
                    Errors = new List<ValidationErrorDetail>()
                    {
                        new ValidationErrorDetail("Error", "Error message")
                    }
                });

            Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(request, new CancellationToken()));

            _repository.Verify(r => r.WithdrawalOrganisation(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
