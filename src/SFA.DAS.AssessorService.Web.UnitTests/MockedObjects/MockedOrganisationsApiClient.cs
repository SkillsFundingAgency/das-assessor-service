using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.UnitTests.MockedObjects
{
    public class MockedOrganisationsApiClient
    {

        public static IOrganisationsApiClient Setup(Mock<ILogger<OrganisationsApiClient>> mockedApiClientLogger)
        {
            var standardOrganisartionSummaries = new List<OrganisationStandardSummary>
            {
                new OrganisationStandardSummary
                {
                    StandardCode = 93
                },
                new OrganisationStandardSummary
                {
                    StandardCode = 92
                },
                new OrganisationStandardSummary
                {
                    StandardCode = 91
                }
            };


            var standards = new List<OrganisationStandardSummary>
            {
                new OrganisationStandardSummary
                {
                    Id = 91
                },
                new OrganisationStandardSummary
                {
                    Id = 92
                },
                new OrganisationStandardSummary
                {
                    Id = 93
                },
                new OrganisationStandardSummary
                {
                    Id = 94
                },
                new OrganisationStandardSummary
                {
                    Id = 95
                },
            };

            var mockHttp = new MockHttpMessageHandler();

            var clientlocal = mockHttp.ToHttpClient();
            clientlocal.BaseAddress = new Uri("https://test.local/");

            mockHttp.When($"/api/ao/assessment-organisations/EPA00001/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standardOrganisartionSummaries));

            var tokenServiceMock = new Mock<IAssessorTokenService>();
            tokenServiceMock
                .Setup(m => m.GetTokenAsync())
                .ReturnsAsync(string.Empty);

            var apiBaseLogger = new Mock<ILogger<ApiClientBase>>();

            var apiClient = new OrganisationsApiClient(clientlocal, tokenServiceMock.Object, apiBaseLogger.Object);

            return apiClient;
        }

    }
}
