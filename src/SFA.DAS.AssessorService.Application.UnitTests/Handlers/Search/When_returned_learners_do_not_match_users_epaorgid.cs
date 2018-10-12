using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_returned_learners_do_not_match_users_epaorgid
    {
        [Test]
        public void Then_non_matching_are_not_returned_if_not_valid_for_epao()
        {
            Mapper.Reset();
            Mapper.Initialize(m => m.CreateMap<Ilr, SearchResult>());
            
            
            var ilrRepository = new Mock<IIlrRepository>();

            ilrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0002", StdCode = 2, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James"}
                });

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            assessmentOrgsApiClient.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<Standard> { new Standard{Title = "Standard Title", Level = 2}});
            assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>(){new StandardOrganisationSummary(){StandardCode = "5"}});
            assessmentOrgsApiClient.Setup(c => c.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new Standard() {Title = "Standard Title", Level = 2});
            
            
            var organisationRepository = new Mock<IOrganisationQueryRepository>();
            organisationRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0001"});

            var certificateRepository = new Mock<ICertificateRepository>();
            certificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());
            
            
            var handler = new SearchHandler(assessmentOrgsApiClient.Object,
                organisationRepository.Object, ilrRepository.Object,
                certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object);

            var result = handler.Handle(new SearchQuery{ Surname = "James", Uln = 1111111111, UkPrn = 12345, Username = "user@name"}, new CancellationToken()).Result;

            result.Count.Should().Be(2);
        }
        
        [Test]
        public void Then_non_matching_are_returned_if_valid_for_epao()
        {
            Mapper.Reset();
            Mapper.Initialize(m => m.CreateMap<Ilr, SearchResult>());
            
            
            var ilrRepository = new Mock<IIlrRepository>();

            ilrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0002", StdCode = 2, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James"}
                });

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            assessmentOrgsApiClient.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<Standard> { new Standard{Title = "Standard Title", Level = 2}});
            assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>(){new StandardOrganisationSummary(){StandardCode = "2"}});
            assessmentOrgsApiClient.Setup(c => c.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new Standard() {Title = "Standard Title", Level = 2});
            
            
            var organisationRepository = new Mock<IOrganisationQueryRepository>();
            organisationRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0001"});

            var certificateRepository = new Mock<ICertificateRepository>();
            certificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());
            
            
            var handler = new SearchHandler(assessmentOrgsApiClient.Object,
                organisationRepository.Object, ilrRepository.Object,
                certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object);

            var result = handler.Handle(new SearchQuery{ Surname = "James", Uln = 1111111111, UkPrn = 12345, Username = "user@name"}, new CancellationToken()).Result;

            result.Count.Should().Be(3);
        }
    }
}