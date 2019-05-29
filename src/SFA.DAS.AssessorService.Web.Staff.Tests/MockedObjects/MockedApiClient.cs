using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
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
            var paginatedResponseApproval =
                new PaginatedList<CertificateSummaryResponse>(
                    certificateSummaryResponses.Where(x => x.Status == "Approved").ToList(), 10, 1,10);
            var paginatedResponseRejections =
                new PaginatedList<CertificateSummaryResponse>(
                    certificateSummaryResponses.Where(x => x.Status == "Rejected").ToList(), 10, 1, 10);
            var paginatedResponseSentForApproval =
                new PaginatedList<CertificateSummaryResponse>(
                    certificateSummaryResponses.Where(x => x.Status == "ToBeApproved").ToList(), 10, 1, 10);

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

            mockHttp.When($"http://localhost:59022/api/v1/certificates/approvals/?pageSize=10&pageIndex=0&status=Submitted&privatelyFundedStatus=Approved")
                .Respond("application/json", JsonConvert.SerializeObject(paginatedResponseApproval));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/approvals/?pageSize=10&pageIndex=0&status=Draft&privatelyFundedStatus=Rejected")
                .Respond("application/json", JsonConvert.SerializeObject(paginatedResponseRejections));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/approvals/?pageSize=0&pageIndex=0&status=ToBeApproved&privatelyFundedStatus=SentForApproval")
                .Respond("application/json", JsonConvert.SerializeObject(paginatedResponseSentForApproval));


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
