using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.CreateApplicationHandlerTests
{
    public class WhenHandlingCreateApplicationRequest
    {
        private CreateApplicationHandler _sut;
        private Mock<IApplyRepository> _mockApplyRepository;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;
        private Mock<IRegisterQueryRepository> _mockRegisterQueryRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;

        private Guid _orgId;
        private Guid _creatingContactId;

        [SetUp]
        public void Arrange()
        {
            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _mockRegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            _orgId = Guid.NewGuid();
            _creatingContactId = Guid.NewGuid();

            _mockOrganisationQueryRepository
                .Setup(m => m.Get(_orgId))
                .ReturnsAsync(new Domain.Entities.Organisation()
                {
                    OrganisationData = new Domain.Entities.OrganisationData()
                    {
                        FHADetails = new ApplyTypes.FHADetails()
                        {
                            FinancialDueDate = DateTime.Today.AddDays(-1)
                        }
                    }
                });

            _mockRegisterQueryRepository
                .Setup(m => m.GetOrganisationTypes())
                .ReturnsAsync(new List<OrganisationType>());

            _mockContactQueryRepository
                .Setup(m => m.GetContactById(_creatingContactId))
                .ReturnsAsync(new Domain.Entities.Contact());

            _sut = new CreateApplicationHandler(_mockOrganisationQueryRepository.Object, _mockRegisterQueryRepository.Object, 
            _mockContactQueryRepository.Object, _mockApplyRepository.Object);
        }

        [Test]
        public async Task AndVersion_ThenFinanicallyExempt()
        {
            //Arrange
            var request = new CreateApplicationRequest()
            {
                OrganisationId = _orgId,
                CreatingContactId = _creatingContactId,
                StandardApplicationType = StandardApplicationTypes.Version,
                ApplySequences = new List<ApplySequence>()
                {
                    new ApplySequence()
                    {
                        SequenceNo = 1,
                        IsActive = true
                    }
                }
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockApplyRepository.Verify(m => m.CreateApplication(It.Is<Domain.Entities.Apply>(x =>
                    x.FinancialReviewStatus == FinancialReviewStatus.Exempt)));
         }

        [Test]
        public async Task AndNotVersion_ThenNotFinanicallyExempt()
        {
            //Arrange
            var request = new CreateApplicationRequest()
            {
                OrganisationId = _orgId,
                CreatingContactId = _creatingContactId,
                StandardApplicationType = StandardApplicationTypes.Full,
                ApplySequences = new List<ApplySequence>()
                {
                    new ApplySequence()
                    {
                        SequenceNo = 1,
                        IsActive = true
                    }
                }
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockApplyRepository.Verify(m => m.CreateApplication(It.Is<Domain.Entities.Apply>(x =>
                    x.FinancialReviewStatus == FinancialReviewStatus.Required)));
        }
    }
}
