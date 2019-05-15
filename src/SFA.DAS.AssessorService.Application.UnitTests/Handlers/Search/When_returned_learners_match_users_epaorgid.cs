﻿using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_returned_learners_match_users_epaorgid
    {
        [Test]
        public void Then_those_learners_are_returned()
        {
            Mapper.Reset();
            Mapper.Initialize(m => m.CreateMap<Ilr, SearchResult>());
            
            
            var ilrRepository = new Mock<IIlrRepository>();

            ilrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 2, FamilyName = "James"},
                    new Ilr{ EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James"}
                });

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            var standardService = new Mock<IStandardService>();
            standardService.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<StandardCollation> { new StandardCollation { Title = "Standard Title", StandardData = new StandardData{ Level = 2}}});
            assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>(){new StandardOrganisationSummary(){StandardCode = "5"}});
            standardService.Setup(c => c.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new StandardCollation {Title = "Standard Title", StandardData = new StandardData{ Level = 2}});
            
            
            var organisationRepository = new Mock<IOrganisationQueryRepository>();
            organisationRepository.Setup(r => r.Get("12345")).ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0001"});

            var certificateRepository = new Mock<ICertificateRepository>();
            certificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());
            
            
            var handler = new SearchHandler(assessmentOrgsApiClient.Object,
                organisationRepository.Object, ilrRepository.Object,
                certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object, standardService.Object);

            var result = handler.Handle(new SearchQuery{ Surname = "James", Uln = 1111111111, EpaOrgId = "12345", Username = "user@name"}, new CancellationToken()).Result;

            result.Count.Should().Be(3);
        }
    }
}