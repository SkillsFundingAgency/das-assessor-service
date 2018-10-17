using System;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.MockedObjects
{
    public class MockedApiClient
    {
        public static ApiClient Setup(Certificate certificate, Mock<ILogger<ApiClient>> apiClientLoggerMock)
        {
            var tokenServiceMock = new Mock<ITokenService>();

            var options = Builder<Option>.CreateListOfSize(10)
                .Build();
            
            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10)
                .Build();

            var certificateSummaryResponses = Builder<CertificateSummaryResponse>.CreateListOfSize(10)
                .All()
                .TheFirst(3)
                .With(x => x.Status = "Approved")
                .TheNext(3)
                .With(x => x.Status = "Rejected")
                .TheNext(4)
                .With(x => x.Status = "ToBeApproved")
                .Build();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));
            /// api / v1 / organisations / organisation /{ id}


            var organisation = Builder<Organisation>.CreateNew()
                .With(q => q.Id = certificate.OrganisationId)
                .With(q => q.EndPointAssessorOrganisationId = "EPA00001")
                .Build();
            
            mockHttp.When($"http://localhost:59022/api/v1/organisations/organisation/{certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(organisation));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/approvals")
                .Respond("application/json", JsonConvert.SerializeObject(certificateSummaryResponses));
                
            mockHttp.When($"http://localhost:59022/api/v1/certificates/options/?stdCode={93}")
                .Respond("application/json", JsonConvert.SerializeObject(options));
            
            
               

            var certificateFirstNameViewModel = new CertificateFirstNameViewModel
            {
                Id = new Guid("1f120837-72d5-40eb-a785-b3936210d47a"),
                FullName = "James Corley",
                FirstName = "James",
                FamilyName = "Corley",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true
            };
            
            mockHttp
                .When(System.Net.Http.HttpMethod.Put, "http://localhost:59022/api/v1/certificates/update")
                .Respond(System.Net.HttpStatusCode.OK, "application/json", "{'status' : 'OK'}");

            var apiClient = new ApiClient(client, apiClientLoggerMock.Object, tokenServiceMock.Object);

            return apiClient;
        }
    }
}
