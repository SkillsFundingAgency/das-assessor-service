using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.AssessorServiceApi
{
    public class WhenSystemProcessesCoverLetter : AssesssorServiceTestBase
    {
        private int _result;

        [SetUp]
        public void Arrange()
        {
            Setup();            

            MockHttp.When("http://localhost:59022/api/v1/certificates/generatebatchnumber")
                .Respond("application/json", "12" ); // Respond with JSON

            _result = AssessorServiceApi.GenerateBatchNumber().Result;
        }

        [Test]
        public void ThenItShouldGenerateABatchNumber()
        {
            _result.Should().Be(12);
        }
    }
}
