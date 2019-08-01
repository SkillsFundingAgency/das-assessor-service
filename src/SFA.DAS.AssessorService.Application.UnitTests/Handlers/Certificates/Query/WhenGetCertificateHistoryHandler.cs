using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificateHistoryHandler
    {                     
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private Mock<IAssessmentOrgsApiClient> _assessmentOrgsApiClientMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<ILogger<GetCertificatesHistoryHandler>> _loggermock;

        private PaginatedList<CertificateSummaryResponse> _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var certificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew().Build());
            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .All()
                .With(q => q.CertificateData = certificateData)
                .With(x => x.CertificateLogs = Builder<CertificateLog>.CreateListOfSize(1).All()
                    .With(q => q.Status = Domain.Consts.CertificateStatus.Submitted).Build().ToList())
                    .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build()
                ).Build().ToList();
                                    
            _certificateRepositoryMock = new Mock<ICertificateRepository>();
            _certificateRepositoryMock.Setup(r => r.GetCertificateHistory(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),It.IsAny<List<string>>()))
                .ReturnsAsync(new PaginatedList<Certificate>(certificates, 40, 1, 10));

            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _contactQueryRepositoryMock.Setup(r => r.GetContact(It.IsAny<string>())).ReturnsAsync(new Contact
            {
                DisplayName = "Test Name"
            });

            _assessmentOrgsApiClientMock = new Mock<IAssessmentOrgsApiClient>();
            _assessmentOrgsApiClientMock.Setup(r => r.GetProvider(It.IsAny<long>()))
                .ReturnsAsync( new Provider
                {
                    ProviderName = "TestProvider",
                    Ukprn = 123456789
                });

            _loggermock = new Mock<ILogger<GetCertificatesHistoryHandler>>();

            var getCertificatesHistoryHandler =
                new GetCertificatesHistoryHandler(_certificateRepositoryMock.Object,
                    _assessmentOrgsApiClientMock.Object, _contactQueryRepositoryMock.Object,
                    _loggermock.Object);

            _result = getCertificatesHistoryHandler.Handle(new GetCertificateHistoryRequest
                {
                    PageIndex = 1,
                    EndPointAssessorOrganisationId = "12345677"
                }, new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Items.Count().Should().BeGreaterOrEqualTo(10);
        }
    }
}
