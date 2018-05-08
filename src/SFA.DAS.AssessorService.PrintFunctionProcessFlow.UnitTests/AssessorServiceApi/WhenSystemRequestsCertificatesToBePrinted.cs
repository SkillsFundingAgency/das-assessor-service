using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.AssessorServiceApi
{
    public class WhenSystemRequestsCertificatesToBePrinted : AssesssorServiceTestBase
    {
        private IEnumerable<CertificateResponse> _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();

            MockHttp.When("http://localhost:59022/api/v1/certificates?status=Submitted")
                .Respond("application/json", JsonConvert.SerializeObject(certificateResponses)); // Respond with JSON

            _result = AssessorServiceApi.GetCertificatesToBePrinted().Result;
        }

        [Test]
        public void ThenItShouldGenerateABatchNumber()
        {
            _result.Count().Should().Be(10);
        }
    }
}
