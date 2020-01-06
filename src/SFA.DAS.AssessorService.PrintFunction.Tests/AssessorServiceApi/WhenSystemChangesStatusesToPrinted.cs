using System.Net.Http;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunction.Tests.AssessorServiceApi
{
    public class WhenSystemChangesStatusesToPrinted : AssessorServiceTestBase
    {
        [TestCase(1, 1)]
        [TestCase(99, 1)]
        [TestCase(100, 1)]
        [TestCase(101, 2)]
        [TestCase(199, 2)]
        [TestCase(200, 2)]
        [TestCase(201, 3)]
        [TestCase(299, 3)]
        [TestCase(300, 3)]
        [TestCase(301, 4)]
        [TestCase(399, 4)]
        [TestCase(400, 4)]
        [TestCase(401, 5)]
        [TestCase(499, 5)]
        [TestCase(500, 5)]
        public async Task ThenItShouldUpdateCertificatesInChunksOf100(int batchSize, int chunksSent)
        {
            Setup();

            var request = MockHttp.When(HttpMethod.Put, "http://localhost:59022/api/v1/certificates/*")
                .Respond(System.Net.HttpStatusCode.OK, "application/json", "{'status' : 'Boo'}");
            
            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(batchSize).Build();
            
            await AssessorServiceApi.ChangeStatusToPrinted(1, certificateResponses);

            Assert.AreEqual(chunksSent, MockHttp.GetMatchCount(request));
        }
    }
}
