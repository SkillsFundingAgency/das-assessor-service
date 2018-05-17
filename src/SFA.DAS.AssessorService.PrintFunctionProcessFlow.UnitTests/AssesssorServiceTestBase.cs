using System;
using Moq;
using RichardSzalay.MockHttp;
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

            AssessorServiceApi = new EpaoImporter.Data.AssessorServiceApi(
                AggregateLogger.Object,
                client,
                new ScheduleConfig
                {
                    DayOfWeek = 1,
                    Hour = 18,
                    Minute = 0
                }
            );
        }
    }
}
