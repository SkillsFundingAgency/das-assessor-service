using Moq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Private;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenStartPrivateCertificateHandler
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private Mock<IOrganisationQueryRepository> _organisationRepositoryMock;
        private Mock<ILogger<StartCertificateHandler>> _loggerMock;
        private Certificate _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();
            
            _loggerMock = new Mock<ILogger<StartCertificateHandler>>();

            var organisation = Builder<Organisation>.CreateNew().Build();

            _organisationRepositoryMock = new Mock<IOrganisationQueryRepository>();
            _organisationRepositoryMock.Setup(q => q.GetByUkPrn(It.IsAny<int>()))
                .Returns(Task.FromResult(organisation));
            

            var certificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                .With(x => x.LearnerFamilyName = "Hawkins")
                .Build());
            var certificate = Builder<Certificate>.CreateNew()
                .With(q => q.Uln = 1111111111)
                .With(q => q.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(q => q.CertificateData = certificateData).Build();
            
            _certificateRepositoryMock = new Mock<ICertificateRepository>();
            _certificateRepositoryMock.Setup(r => r.GetPrivateCertificate(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(certificate));
                               
            var startPrivateCertificateHandler =
                new StartPrivateCertificateHandler(_certificateRepositoryMock.Object, _organisationRepositoryMock.Object,
                    _loggerMock.Object
                    );

            _result = startPrivateCertificateHandler.Handle(new StartCertificatePrivateRequest
                {
                    Uln = 1111111111,
                    EndPointAssessorOrganisationId = "EPA00001",
                    LastName = "Hawkins",
                    UkPrn = 99999999,
                    Username = "TestUser"
                }, new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Uln.Should().Be(1111111111);
        }
    }
}
