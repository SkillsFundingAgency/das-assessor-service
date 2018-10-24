using FizzWare.NBuilder;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.PrintFunction.Tests.AssessorServiceApi
{
    public class WhenSystemProcessesCoverLetter : AssessorServiceTestBase
    {
        private BatchLogResponse _result;
        private BatchLogResponse _expected;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _expected = Builder<BatchLogResponse>.CreateNew()
                .With(q => q.BatchNumber = 12)
                .Build();

            MockHttp.When("http://localhost:59022/api/v1/batches/latest")
                .Respond("application/json", JsonConvert.SerializeObject(_expected)); // Respond with JSON

            _result = AssessorServiceApi.GetCurrentBatchLog().Result;
        }

        [Test]
        public void ThenItShouldGenerateABatchNumber()
        {
            _result.BatchNumber.Should().Be(12);
        }
    }
}
