using System.Net.Http;
using FizzWare.NBuilder;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunction.Tests.AssessorServiceApi
{
    public class WhenSystemChangesStatusesToPrinted : AssessorServiceTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();
            
            MockHttp.When(HttpMethod.Put, "http://localhost:59022/api/v1/certificates/*")                         
                .Respond(System.Net.HttpStatusCode.OK, "application/json", "{'status' : 'Boo'}");

            AssessorServiceApi.ChangeStatusToPrinted(1, certificateResponses).ConfigureAwait(false);
        }

        [Test]
        public void ThenItShouldUpdateCertificates()
        {
            MockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
