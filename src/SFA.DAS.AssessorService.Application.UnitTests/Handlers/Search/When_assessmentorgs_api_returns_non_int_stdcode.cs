using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_assessmentorgs_api_return_contains_non_int : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            
            Setup();
        }
        
        [Test]
        public void Then_an_exception_is_not_thrown()
        {
            AssessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0050"))
                .ReturnsAsync(new List<StandardOrganisationSummary>
                {
                    new StandardOrganisationSummary {StandardCode = "12"},
                    new StandardOrganisationSummary {StandardCode = "13a"},
                    new StandardOrganisationSummary {StandardCode = "14"}
                });

            SearchHandler
                .Handle(new SearchQuery() {Surname = "smith", UkPrn = 99999, Uln = 12345, Username = "dave"},
                    new CancellationToken()).Wait();
        }
    }
}