using System.Linq;
using System.Net.Http;
using FizzWare.NBuilder;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.AssessorServiceApi
{
    public class WhenSystemChangesStatusesToPrinted : AssesssorServiceTestBase
    {
        private int _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();
            var certificateStatuses = certificateResponses.Select(
                q => new CertificateStatus
                {
                    CertificateReference = q.CertificateReference,
                    Status = Domain.Consts.CertificateStatus.Printed
                }).ToList();

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
