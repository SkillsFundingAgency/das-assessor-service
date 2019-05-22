﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    public class SearchHandlerTestBase
    {
        protected SearchHandler SearchHandler;
        protected Mock<ICertificateRepository> CertificateRepository;
        protected Mock<IIlrRepository> IlrRepository;
        protected Mock<IAssessmentOrgsApiClient> AssessmentOrgsApiClient;
        protected Mock<IContactQueryRepository> ContactRepository;
        protected Mock<IStandardService> StandardService;

        public void Setup()
        {
            MappingBootstrapper.Initialize();

            AssessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            StandardService = new Mock<IStandardService>();

            StandardService.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<StandardCollation> { new StandardCollation{Title = "Standard Name 12", StandardData = new StandardData{Level = 2}}, 
                    new StandardCollation{Title = "Standard Name 13", StandardData = new StandardData{Level = 3}} });

            AssessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>
                {
                    new StandardOrganisationSummary {StandardCode = "12"},
                    new StandardOrganisationSummary {StandardCode = "13"}
                });
                      StandardService.Setup(c => c.GetStandard(12))
                .ReturnsAsync(new StandardCollation { Title = "Standard Name 12", StandardData = new StandardData{Level = 2} });
            StandardService.Setup(c => c.GetStandard(13))
                .ReturnsAsync(new StandardCollation { Title = "Standard Name 13", StandardData = new StandardData{Level = 3}});


            var orgQueryRepo = new Mock<IOrganisationQueryRepository>();
            orgQueryRepo.Setup(r => r.Get("12345"))
                .ReturnsAsync(new Organisation() {EndPointAssessorOrganisationId = "EPA001"});
            
            orgQueryRepo.Setup(r => r.Get("99999"))
                .ReturnsAsync(new Organisation() {EndPointAssessorOrganisationId = "EPA0050"});

            IlrRepository = new Mock<IIlrRepository>();
            

            CertificateRepository = new Mock<ICertificateRepository>();

            ContactRepository = new Mock<IContactQueryRepository>();
            SearchHandler = new SearchHandler(AssessmentOrgsApiClient.Object, orgQueryRepo.Object, IlrRepository.Object,
                CertificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, ContactRepository.Object, StandardService.Object);
        }
    }
}