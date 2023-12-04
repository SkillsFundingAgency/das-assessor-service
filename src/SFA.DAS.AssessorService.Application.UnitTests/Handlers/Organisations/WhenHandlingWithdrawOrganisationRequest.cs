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
        private Mock<IOrganisationStandardRepository> _orgStandardRepository;
        private Mock<IApplyRepository> _applyRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private WithdrawOrganisationHandler _handler;
        private Mock<IEpaOrganisationValidator> _validator;

        private Guid _applicationId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _orgStandardRepository = new Mock<IOrganisationStandardRepository>();
            _applyRepository = new Mock<IApplyRepository>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new WithdrawOrganisationHandler( _validator.Object, _orgStandardRepository.Object, _applyRepository.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task ThenWithdrawOrganisationRepoIsCalled()
        {
            // arrange 
            var withdrawDate = new DateTime(2021, 12, 1);

            var request = new WithdrawOrganisationRequest()
            {
                ApplicationId = _applicationId,
                EndPointAssessorOrganisationId = "EPA0123",
                WithdrawalDate = withdrawDate,
                UpdatedBy = "UPDATER"
            };

            _validator.Setup(m => m.ValidatorWithdrawOrganisationRequest(request)).Returns(new ValidationResponse());

            var result = await _handler.Handle(request, new CancellationToken());

            _orgStandardRepository.Verify(r => r.WithdrawOrganisation("EPA0123", withdrawDate));
            _unitOfWork.Verify(r => r.Commit());
        }

        [Test]
        public async Task ThenApplyRepoIsCalled()
        {
            // arrange 
            var withdrawDate = new DateTime(2021, 12, 1);

            var request = new WithdrawOrganisationRequest()
            {
                ApplicationId = _applicationId,
                EndPointAssessorOrganisationId = "EPA0123",
                WithdrawalDate = withdrawDate,
                UpdatedBy = "UPDATER"
            };

            _validator.Setup(m => m.ValidatorWithdrawOrganisationRequest(request)).Returns(new ValidationResponse());

            var result = await _handler.Handle(request, new CancellationToken());

            _applyRepository.Verify(r => r.DeclineAllApplicationsForOrgansiation(_applicationId, "EPA0123", "UPDATER"));
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

            _orgStandardRepository.Verify(r => r.WithdrawOrganisation(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
