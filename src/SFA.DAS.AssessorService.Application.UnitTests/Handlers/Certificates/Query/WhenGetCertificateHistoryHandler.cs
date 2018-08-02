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
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificateHistoryHandler
    {                     
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private Mock<IAssessmentOrgsApiClient> _assessmentOrgsApiClientMock;
        private Mock<ILogger<GetCertificatesHistoryHandler>> _loggermock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;

        private PaginatedList<CertificateHistoryResponse> _result;

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
            _certificateRepositoryMock.Setup(r => r.GetCertificateHistory(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new PaginatedList<Certificate>(certificates, 40, 1, 10)));

            _assessmentOrgsApiClientMock = new Mock<IAssessmentOrgsApiClient>();
            _assessmentOrgsApiClientMock.Setup(r => r.GetProvider(It.IsAny<long>()))
                .Returns(Task.FromResult(new Provider
                {
                    ProviderName = "TestProvider",
                    Ukprn = 123456789
                }));

            _loggermock = new Mock<ILogger<GetCertificatesHistoryHandler>>();

            var getCertificatesHitoryHandler =
                new GetCertificatesHistoryHandler(_certificateRepositoryMock.Object,
                    _assessmentOrgsApiClientMock.Object,
                    _loggermock.Object);

            _result = getCertificatesHitoryHandler.Handle(new GetCertificateHistoryRequest
                {
                    PageIndex = 1,
                    Username = "TestUser"
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
