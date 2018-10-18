using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Web.UnitTests.MockedObjects
{
    public class MockedAssessmentOrgsApiClient
    {
        public static IAssessmentOrgsApiClient Setup(Mock<ILogger<CertificateApiClient>> mockedApiClientLogger)
        {
            var standardOrganisartionSummaries = new List<StandardOrganisationSummary>
            {
                new StandardOrganisationSummary
                {
                    StandardCode = "93"
                },
                new StandardOrganisationSummary
                {
                    StandardCode = "92"
                },
                new StandardOrganisationSummary
                {
                    StandardCode = "91"
                }
            };


            var standards = new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "91",
                    Level = 2,
                    Title = "Test Title 1"
                },
                new StandardSummary
                {
                    Id = "92",
                    Level = 3,
                    Title = "Test Title 2"
                },
                new StandardSummary
                {
                    Id = "93",
                    Level = 5,
                    Title = "Test Title 3"
                },
                new StandardSummary
                {
                    Id = "94",
                    Level = 2,
                    Title = "Test Title 4"
                },
                new StandardSummary
                {
                    Id = "95",
                    Level = 2,
                    Title = "Test Title 5"
                },
            };

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("https://findapprenticeshiptraining-api.sfa.bis.gov.uk/");

            mockHttp.When($"/assessment-organisations/EPA00001/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standardOrganisartionSummaries));

            mockHttp.When($"https://findapprenticeshiptraining-api.sfa.bis.gov.uk/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standards));

            var apiClient = new AssessmentOrgsApiClient(client);

            return apiClient;
        }
    }
}
