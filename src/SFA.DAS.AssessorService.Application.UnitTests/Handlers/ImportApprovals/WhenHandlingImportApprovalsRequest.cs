using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Approvals;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ImportApprovals
{
    [TestFixture]
    public class WhenHandlingImportApprovalsRequest
    {
        private Mock<ILogger<ImportApprovalsHandler>> _loggerMock;
        private Mock<IApprovalsExtractRepository> _approvalsExtractRepoMock;
        private Mock<ISettingRepository> _settingRepositoryMock;
        private Mock<IOuterApiService> _outerApiServiceMock;
        private ImportApprovalsHandler _sut;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ImportApprovalsHandler>>();
            _approvalsExtractRepoMock = new Mock<IApprovalsExtractRepository>();
            _settingRepositoryMock = new Mock<ISettingRepository>();
            _outerApiServiceMock = new Mock<IOuterApiService>();
            _sut = new ImportApprovalsHandler(_loggerMock.Object, _approvalsExtractRepoMock.Object, _settingRepositoryMock.Object, _outerApiServiceMock.Object);
        }

        [Test]
        [Ignore("Dev in progress")]
        public async Task Then_Get_Date_Of_Latest_Record_In_ApprovalsExtract()
        {
            // Arrange.

            // Act.

            await _sut.Handle(new ImportApprovalsRequest(), new CancellationToken());

            // Assert.

            _approvalsExtractRepoMock.Verify(o => o.GetLatestExtractTimestamp());            
        }

        [Test]
        [Ignore("Dev in progress")]
        public async Task Then_Get_Tolerance_Config_From_Settings()
        {
            // Arrange.

            // The handler only gets tolerance config if a date was returned by GetLatestExtractTimestamp

            _approvalsExtractRepoMock.Setup(m => m.GetLatestExtractTimestamp()).ReturnsAsync(DateTime.UtcNow);

            // Act.

            await _sut.Handle(new ImportApprovalsRequest(), new CancellationToken());

            // Assert.

            _settingRepositoryMock.Verify(o => o.GetSetting(ImportApprovalsHandler.TOLERANCE_SETTING_NAME));
        }

        [Test]
        [Ignore("Dev in progress")]
        public async Task Then_Get_BatchSize_Config_From_Settings()
        {
            // Arrange.

            _outerApiServiceMock.Setup(m => m.GetAllLearners(It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Application.Infrastructure.OuterApi.GetAllLearnersResponse() 
                { 
                    BatchNumber = 1, BatchSize = 1, TotalNumberOfBatches = 1, 
                    Learners = new System.Collections.Generic.List<Application.Infrastructure.OuterApi.Learner>() 
                    { 
                        new Application.Infrastructure.OuterApi.Learner() { ApprenticeshipId = 1, FirstName = "Test", LastName = "Test1" } 
                    } 
                });

            // Act.

            await _sut.Handle(new ImportApprovalsRequest(), new CancellationToken());

            // Assert.

            _settingRepositoryMock.Verify(o => o.GetSetting(ImportApprovalsHandler.BATCHSIZE_SETTING_NAME));
        }

        [Test]
        [Ignore("Dev in progress")]
        public async Task Then_Get_Learners_Batches_From_AssessorsOuterApi()
        {
            // Arrange.


            // Act.

            await _sut.Handle(new ImportApprovalsRequest(), new CancellationToken());

            // Assert.
        }

        [Test]
        public void Then_Insert_Learners_Batches_Into_ApprovalsExtractTable()
        {
        }

        [Test]
        public void Then_Execute_PopulateLearner_Stored_Procedure()
        {
        }
    }
}
