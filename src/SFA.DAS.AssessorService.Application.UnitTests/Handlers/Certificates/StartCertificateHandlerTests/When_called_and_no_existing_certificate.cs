using System;
using System.Threading;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    [TestFixture]
    public class When_called_and_no_existing_certificate
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private StartCertificateHandler _startCertificateHandler;
        private Guid _organisationId;
        private Certificate _returnedCertificate;
        private CertificateData _certificateData;

        [SetUp]
        public void Arrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();           
            _certificateRepository.Setup(r => r.GetCertificate(1111111111, 30)).ReturnsAsync(default(Certificate));
           
            _certificateRepository.Setup(r => r.New(It.IsAny<Certificate>()))
                .ReturnsAsync(new Certificate() {CertificateReferenceId = 10000});

            var ilrRepository = new Mock<IIlrRepository>();
            ilrRepository.Setup(r => r.Get(1111111111, 30)).ReturnsAsync(new Ilr()
            {
                GivenNames = "Dave",
                FamilyName = "Smith",
                StdCode = 30,
                LearnStartDate = new DateTime(2016, 01, 09),
                UkPrn = 12345678
            });

            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            _organisationId = Guid.NewGuid();

            organisationQueryRepository.Setup(r => r.GetByUkPrn(88888888)).ReturnsAsync(new Organisation() { Id = _organisationId});

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            var standardService = new Mock<IStandardService>();

            standardService.Setup(c => c.GetStandard(30))
                .ReturnsAsync(new StandardCollation()
                {
                    Title = "Standard Name",
                    StandardData = new StandardData
                    {
                        EffectiveFrom = new DateTime(2016,09,01)
                    }
                });
            assessmentOrgsApiClient.Setup(c => c.GetProvider(It.IsAny<long>()))
                .ReturnsAsync(new Provider {ProviderName = "A Provider"});

            _startCertificateHandler = new StartCertificateHandler(_certificateRepository.Object,
                ilrRepository.Object, assessmentOrgsApiClient.Object,
                organisationQueryRepository.Object, new Mock<ILogger<StartCertificateHandler>>().Object, standardService.Object);

            _returnedCertificate = _startCertificateHandler
                .Handle(
                    new StartCertificateRequest()
                    {
                        StandardCode = 30,
                        UkPrn = 88888888,
                        Uln = 1111111111,
                        Username = "user"                        
                    }, new CancellationToken()).Result;
        }

        [Test]
        public void Then_a_new_certificate_is_created()
        {
            _certificateRepository.Verify(r => r.New(It.Is<Certificate>(c =>
                c.Uln == 1111111111 &&
                c.StandardCode == 30 &&
                c.ProviderUkPrn == 12345678 &&
                c.OrganisationId == _organisationId &&
                c.CreatedBy == "user" &&
                c.Status == Domain.Consts.CertificateStatus.Draft &&
                c.CertificateReference == "")));
        }


        [Test]
        public void Then_certificate_exists_with_delete_status_then_update_certificate()
        {
            //Arrange
            _certificateData = Builder<CertificateData>.CreateNew()
             .With(ecd => ecd.LearnerGivenNames = "Dave")
             .With(ecd => ecd.LearnerFamilyName = "Smith")
             .With(ecd => ecd.LearningStartDate = new DateTime(2016, 01, 09))
             .Build();

            _certificateRepository.Setup(r => r.GetCertificate(1111111111, 30)).ReturnsAsync(new Certificate()
            {
                CertificateReferenceId = 10000,
                Status = CertificateStatus.Deleted,
                CertificateData = JsonConvert.SerializeObject(_certificateData)
            });

            _certificateRepository.Setup(r => r.Update(It.IsAny<Certificate>(), It.IsAny<string>(), It.IsAny<string>(),
             It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(new Certificate()
             { CertificateReferenceId = 10000, Status = CertificateStatus.Deleted, CertificateData = JsonConvert.SerializeObject(_certificateData) });

            //Act
            _returnedCertificate = _startCertificateHandler
              .Handle(
                  new StartCertificateRequest()
                  {
                      StandardCode = 30,
                      UkPrn = 88888888,
                      Uln = 1111111111,
                      Username = "user"
                  }, new CancellationToken()).Result;
          

            //Assert
            _certificateRepository.Verify(v => v.Update(
              It.IsAny<Certificate>(),
              It.IsAny<string>(), It.IsAny<string>(),
               It.IsAny<bool>(), It.IsAny<string>()),
              Times.Once);
        }      
    }    
}