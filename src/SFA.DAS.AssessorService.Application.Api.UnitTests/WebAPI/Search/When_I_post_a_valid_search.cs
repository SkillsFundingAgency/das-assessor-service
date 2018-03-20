using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Search
{
    [TestFixture]
    public class When_I_post_a_valid_search
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Ilr, SearchResult>();
            });

            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            organisationQueryRepository.Setup(r => r.GetByUkPrn(It.IsAny<int>()))
                .ReturnsAsync(new OrganisationResponse() { EndPointAssessorOrganisationId = "1" });

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("1"))
                .ReturnsAsync(new List<StandardOrganisationSummary>
                {
                    new StandardOrganisationSummary {StandardCode = "20"},
                    new StandardOrganisationSummary {StandardCode = "30"}
                });

            assessmentOrgsApiClient.Setup(c => c.GetStandard("20"))
                .ReturnsAsync(new Standard { Title = "Standard Name 20" });

            var ilrRepo = new Mock<IIlrRepository>();
            ilrRepo.Setup(r => r.Search(It.IsAny<SearchRequest>())).ReturnsAsync(new List<Ilr> { new Ilr() { FamilyName = "Smith", StdCode = "20" } });

            var controller = new SearchController(assessmentOrgsApiClient.Object,
                organisationQueryRepository.Object, ilrRepo.Object);

            _result = controller.Search(new SearchQuery
            {
                Surname = "Smith",
                UkPrn = 88888888,
                Uln = 1111111111,
                Username = "user"
            }).Result;
        }

        [Test]
        public void Then_OK_should_be_returned()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void Then_model_should_contain_search_results()
        {
            ((OkObjectResult) _result).Value.Should().BeOfType<List<SearchResult>>();
        }

        [Test]
        public void Then_search_results_should_be_correct()
        {
            var searchResults = ((OkObjectResult) _result).Value as List<SearchResult>;
            searchResults.Count.Should().Be(1);
            searchResults.First().FamilyName.Should().Be("Smith");
            searchResults.First().Standard.Should().Be("Standard Name 20");
        }
    }
}