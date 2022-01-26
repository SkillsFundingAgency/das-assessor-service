using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    public class SearchHandlerTestBase
    {
        protected SearchHandler SearchHandler;
        protected Mock<ICertificateRepository> CertificateRepository;
        protected Mock<ILearnerRepository> LearnerRepository;
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected Mock<IContactQueryRepository> ContactRepository;
        protected Mock<IStandardService> StandardService;

        public void Setup()
        {
            MappingBootstrapper.Initialize();

            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            StandardService = new Mock<IStandardService>();

            StandardService.Setup(c => c.GetAllStandardVersions())
                .ReturnsAsync(new List<Standard> { new Standard{LarsCode = 12, Title = "Standard Name 12", Level = 2, StandardUId = "ST012_1.0", Version = "1.0"},
                    new Standard{LarsCode = 13, Title = "Standard Name 13", Level=3, StandardUId = "ST013_1.0", Version = "1.0"}});

            StandardService.Setup(s => s.GetEPAORegisteredStandardVersions(It.IsAny<string>(), null))
                .ReturnsAsync(new List<OrganisationStandardVersion> { new OrganisationStandardVersion { Title = "Standard 12", Version = "1.0", LarsCode = 12 },
                                                          new OrganisationStandardVersion { Title = "Standard 13", Version = "1.0", LarsCode = 13 } });

            RegisterQueryRepository.Setup(c => c.GetAllOrganisationStandardByOrganisationId("EPA001"))
                .ReturnsAsync(new List<OrganisationStandardSummary>
                {
                    new OrganisationStandardSummary {StandardCode = 12},
                    new OrganisationStandardSummary {StandardCode = 13}
                });

            var orgQueryRepo = new Mock<IOrganisationQueryRepository>();
            orgQueryRepo.Setup(r => r.Get("12345"))
                .ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA001" });

            orgQueryRepo.Setup(r => r.Get("99999"))
                .ReturnsAsync(new Organisation() { EndPointAssessorOrganisationId = "EPA0050" });

            LearnerRepository = new Mock<ILearnerRepository>();


            CertificateRepository = new Mock<ICertificateRepository>();

            ContactRepository = new Mock<IContactQueryRepository>();
            SearchHandler = new SearchHandler(orgQueryRepo.Object, LearnerRepository.Object,
                CertificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, ContactRepository.Object, StandardService.Object);
        }
    }
}