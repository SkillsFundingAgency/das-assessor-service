using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
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

        public void Setup()
        {
            MappingBootstrapper.Initialize();

            AssessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            AssessmentOrgsApiClient.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<Standard> { new Standard{Title = "Standard Name 12", Level = 2}, new Standard{Title = "Standard Name 13", Level = 3} });
            AssessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>
                {
                    new StandardOrganisationSummary {StandardCode = "12"},
                    new StandardOrganisationSummary {StandardCode = "13"}
                });
            AssessmentOrgsApiClient.Setup(c => c.GetStandard(12))
                .ReturnsAsync(new Standard {Title = "Standard Name 12", Level = 2});
            AssessmentOrgsApiClient.Setup(c => c.GetStandard(13))
                .ReturnsAsync(new Standard {Title = "Standard Name 13", Level = 3});

            var orgQueryRepo = new Mock<IOrganisationQueryRepository>();
            orgQueryRepo.Setup(r => r.GetByUkPrn(12345))
                .ReturnsAsync(new Organisation() {EndPointAssessorOrganisationId = "EPA001"});
            
            orgQueryRepo.Setup(r => r.GetByUkPrn(99999))
                .ReturnsAsync(new Organisation() {EndPointAssessorOrganisationId = "EPA0050"});

            IlrRepository = new Mock<IIlrRepository>();
            

            CertificateRepository = new Mock<ICertificateRepository>();

            ContactRepository = new Mock<IContactQueryRepository>();
            SearchHandler = new SearchHandler(AssessmentOrgsApiClient.Object, orgQueryRepo.Object, IlrRepository.Object,
                CertificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, ContactRepository.Object);
        }
    }
}