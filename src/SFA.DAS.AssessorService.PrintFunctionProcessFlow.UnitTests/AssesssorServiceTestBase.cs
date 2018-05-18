using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class AssesssorServiceTestBase
    {
        protected Mock<IAggregateLogger> AggregateLogger;
        protected MockHttpMessageHandler MockHttp;
        protected EpaoImporter.Data.AssessorServiceApi AssessorServiceApi;

        protected void Setup()
        {
            AggregateLogger = new Mock<IAggregateLogger>();

            MockHttp = new MockHttpMessageHandler();
            
            var client = MockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            var schedulingConfigurationServiceMock = new Mock<ISchedulingConfigurationService>();

            schedulingConfigurationServiceMock.Setup(q => q.GetSchedulingConfiguration())
                .Returns(Task.FromResult(new ScheduleConfiguration
                {
                    Data = JsonConvert.SerializeObject(new SchedulingConfiguraionData
                    {
                        DayOfWeek = (int )DateTime.Now.DayOfWeek,
                        Hour = DateTime.Now.Hour -1,
                        Minute = DateTime.Now.Minute
                    })
                }));

            AssessorServiceApi = new EpaoImporter.Data.AssessorServiceApi(
                AggregateLogger.Object,
                schedulingConfigurationServiceMock.Object,
                client
            );
        }
    }
}
