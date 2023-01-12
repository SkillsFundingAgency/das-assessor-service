using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.DeleteApplicationsHandlerTests
{
    public class WhenHandlingDeleteApplicationsRequest
    {
        private DeleteApplicationsHandler _sut;
        private Mock<IApplyRepository> _mockApplyRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<DeleteApplicationsHandler>> _mockLogger;

        private Guid _applicationId1, _applicationId2;
        private Guid _contactId;

        [SetUp]
        public void Arrange()
        {
            _mockApplyRepository = new Mock<IApplyRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<DeleteApplicationsHandler>>();

            _applicationId1 = Guid.NewGuid();
            _applicationId2 = Guid.NewGuid();
            _contactId = Guid.NewGuid();

            _sut = new DeleteApplicationsHandler(_mockApplyRepository.Object, _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Test]
        public async Task ThenApplicationsAreDeleted()
        {
            //Arrange
            var request = new DeleteApplicationsRequest()
            {
                DeletingContactId = _contactId,
                ApplicationIds = new List<Guid>() { _applicationId1, _applicationId2 }
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockApplyRepository.Verify(m => m.DeleteApplication(_applicationId1, _contactId.ToString()));
            _mockApplyRepository.Verify(m => m.DeleteApplication(_applicationId2, _contactId.ToString()));
        }

        [Test]
        public async Task AndNoContactIdIsProvided_ThenApplicationsAreDeletedUsingSystem()
        {
            //Arrange
            var request = new DeleteApplicationsRequest()
            {
                DeletingContactId = null,
                ApplicationIds = new List<Guid>() { _applicationId1, _applicationId2 }
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockApplyRepository.Verify(m => m.DeleteApplication(_applicationId1, "System"));
            _mockApplyRepository.Verify(m => m.DeleteApplication(_applicationId2, "System"));
        }
    }
}
