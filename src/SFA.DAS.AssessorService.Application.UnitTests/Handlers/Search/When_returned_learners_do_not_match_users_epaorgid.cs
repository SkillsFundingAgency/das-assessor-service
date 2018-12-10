using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_returned_learners_do_not_match_users_epaorgid
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IIlrRepository> _ilrRepository;
        private Mock<IAssessmentOrgsApiClient> _assessmentOrgsApiClient;
        private Mock<IContactQueryRepository> _contactRepository;
        private Mock<ISubmissionEventProviderApiClient> _providerEventsApiClientMock;
        private Mock<IOrganisationQueryRepository> _organisationRepository;

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(m => m.CreateMap<Ilr, SearchResult>());
            _certificateRepository = new Mock<ICertificateRepository>();
            _assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            _contactRepository = new Mock<IContactQueryRepository>();
            _organisationRepository = new Mock<IOrganisationQueryRepository>();
            _ilrRepository = new Mock<IIlrRepository>();
            _ilrRepository.Setup(r => r.SearchForLearnerByUlnAndFamilyName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr
                    {
                        Uln = 1111111111, EpaOrgId = "EPA0001", StdCode = 1, FamilyName = "James",
                        CompletionStatus = CompletionStatus.Completed
                    },
                    new Ilr
                    {
                        Uln = 1111111111, EpaOrgId = "EPA0002", StdCode = 2, FamilyName = "James",
                        CompletionStatus = CompletionStatus.Completed
                    },
                    new Ilr
                    {
                        Uln = 1111111111, EpaOrgId = "EPA0001", StdCode = 3, FamilyName = "James",
                        CompletionStatus = CompletionStatus.Completed
                    }
                });

            _organisationRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation()
                { EndPointAssessorOrganisationId = "EPA0001" });

            _certificateRepository.Setup(r => r.GetSubmittedAndDraftCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate
                    {
                        CertificateReference = "00010001",
                        StandardCode = 1,
                        CertificateData =
                            JsonConvert.SerializeObject(new CertificateData {OverallGrade = "Distinction"})

                    }
                });
            _certificateRepository.Setup(r => r.GetCertificateLogsFor(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CertificateLog>
                {
                    new CertificateLog
                    {
                        Status = CertificateStatus.Submitted,
                        Username = "username",
                        EventTime = DateTime.UtcNow

                    }
                });

            _contactRepository.Setup(r => r.GetContact("user@name")).ReturnsAsync(new Contact
            {
                OrganisationId = Guid.Parse("B79EF91B-DB69-445F-B1EC-007B8C1B5D0D")
            });


            _ilrRepository.Setup(r => r.RefreshFromSubmissionEventData(It.IsAny<Guid>(), It.IsAny<SubmissionEvent>()))
                .Returns(Task.CompletedTask);
            _ilrRepository.Setup(r => r.MarkAllUpToDate(It.IsAny<long>())).Returns(Task.CompletedTask);

            _providerEventsApiClientMock = new Mock<ISubmissionEventProviderApiClient>();
            _providerEventsApiClientMock.Setup(c => c.GetLatestLearnerEventForStandards(1111111111, 0))
                .ReturnsAsync(new List<SubmissionEvent>
                {
                    new SubmissionEvent
                    {
                        FamilyName = "James",
                        Id = 1,
                        StandardCode = 1,
                        Uln = 1111111111
                    }
                });
        }

        [Test]
        public void Then_non_matching_are_not_returned_if_not_valid_for_epao()
        {
            _assessmentOrgsApiClient.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<Standard> {new Standard {Title = "Standard Title", Level = 2}});
            _assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>()
                    {new StandardOrganisationSummary() {StandardCode = "5"}});
            _assessmentOrgsApiClient.Setup(c => c.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new Standard() {Title = "Standard Title", Level = 2});

            var handler = new SearchHandler(_assessmentOrgsApiClient.Object,
                _organisationRepository.Object, _ilrRepository.Object,
                _certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object,
                _contactRepository.Object, _providerEventsApiClientMock.Object);
            
            var result = handler
                .Handle(new SearchQuery {Surname = "James", Uln = 1111111111, UkPrn = 12345, Username = "user@name"},
                    new CancellationToken()).Result;

            result.Count.Should().Be(2);
        }

        [Test]
        public void Then_non_matching_are_returned_if_valid_for_epao()
        {
            _assessmentOrgsApiClient.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<Standard> { new Standard{Title = "Standard Title", Level = 2}});
            _assessmentOrgsApiClient.Setup(c => c.FindAllStandardsByOrganisationIdAsync("EPA0001"))
                .ReturnsAsync(new List<StandardOrganisationSummary>(){new StandardOrganisationSummary(){StandardCode = "2"}});
            _assessmentOrgsApiClient.Setup(c => c.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new Standard() {Title = "Standard Title", Level = 2});
           
            var handler = new SearchHandler(_assessmentOrgsApiClient.Object,
                _organisationRepository.Object, _ilrRepository.Object,
                _certificateRepository.Object, new Mock<ILogger<SearchHandler>>().Object, new Mock<IContactQueryRepository>().Object, _providerEventsApiClientMock.Object);

            var result = handler.Handle(new SearchQuery{ Surname = "James", Uln = 1111111111, UkPrn = 12345, Username = "user@name"}, new CancellationToken()).Result;

            result.Count.Should().Be(3);
        }
    }
}