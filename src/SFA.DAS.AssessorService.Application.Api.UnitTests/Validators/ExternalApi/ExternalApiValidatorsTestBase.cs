using FizzWare.NBuilder;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi
{
    public class ExternalApiValidatorsTestBase
    {
        public Mock<ICertificateRepository> CertificateRepositoryMock { get; }
        public Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock { get; }
        public Mock<ILearnerRepository> LearnerRepositoryMock { get; }
        public Mock<IStandardService> StandardServiceMock { get; }

        public ExternalApiValidatorsTestBase()
        {
            CertificateRepositoryMock = SetupCertificateRepositoryMock();
            OrganisationQueryRepositoryMock = SetupOrganisationQueryRepositoryMock();
            LearnerRepositoryMock = SetupLearnerRepositoryMock();
            StandardServiceMock = SetupStandardServiceMock();
        }

        private static Mock<ICertificateRepository> SetupCertificateRepositoryMock()
        {
            var certificateRepositoryMock = new Mock<ICertificateRepository>();

            // Having a range of certificates for 2 EPAO's (with a shared standard) allows us to test the suite of validation rules
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 1)).ReturnsAsync(GenerateCertificate(1234567890, 1, "test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 98)).ReturnsAsync(GenerateCertificate(1234567890, 98, "test", CertificateStatus.Deleted, new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 1)).ReturnsAsync(GenerateCertificate(9999999999, 1, "test", CertificateStatus.Printed, new Guid("99999999999999999999999999999999")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 99)).ReturnsAsync(GenerateCertificate(1234567890, 99, "Test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));

            // This is simulating a Certificate that started it's life on the Web App, but was never submitted
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 99)).ReturnsAsync(GeneratePartialCertificate(1234567890, 99, "test", new Guid("12345678123456781234567812345678"), null, CertificateStatus.Draft));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 99)).ReturnsAsync(GeneratePartialCertificate(9999999999, 99, "test", new Guid("99999999999999999999999999999999"), CertificateGrade.Fail, CertificateStatus.Submitted));

            // These allow us to test EPAs, which is the initial stage of a Certificate
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 101)).ReturnsAsync(GenerateEpaCertificate(1234567890, 101, "test", new Guid("12345678123456781234567812345678"), true));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 101)).ReturnsAsync(GenerateEpaCertificate(9999999999, 101, "test", new Guid("99999999999999999999999999999999"), false));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9876543210, 101)).ReturnsAsync(GenerateEpaCertificate(9876543210, 101, "test", new Guid("99999999999999999999999999999999"), false));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9876543211, 101)).ReturnsAsync(GenerateEpaCertificate(9876543211, 101, "test", new Guid("99999999999999999999999999999999"), false, overallGrade: "Pass"));

            // This allows us to test, retrieving by ilr/learner data if the calling organisation was not the one that created it
            certificateRepositoryMock.Setup(q => q.GetCertificateByUlnOrgIdLastnameAndStandardCode(1234567890, "99999999", "Test", 1))
                .ReturnsAsync((Certificate)null);
            certificateRepositoryMock.Setup(q => q.GetCertificateByUlnOrgIdLastnameAndStandardCode(1234567890, "12345678", "Test", 1))
                .ReturnsAsync(GenerateCertificate(1234567890, 1, "Test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));

            // This is for SV-1255 testing standard and option validation on updates 
            certificateRepositoryMock.Setup(q => q.GetCertificate(5555555555, 55)).ReturnsAsync(GenerateCertificate(5555555555, 55, "Test", CertificateStatus.Draft, new Guid("55555555555555555555555555555555")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(5555555556, 55)).ReturnsAsync(GenerateCertificate(5555555556, 55, "Test", CertificateStatus.Draft, new Guid("55555555555555555555555555555555")));
            // SV-1260
            certificateRepositoryMock.Setup(q => q.GetCertificate(3333333333, 1)).ReturnsAsync(GenerateCertificate(3333333333, 1, "test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(4343434343, 1)).ReturnsAsync(GenerateCertificate(4343434343, 1, "test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));

            return certificateRepositoryMock;
        }

        private static Mock<IOrganisationQueryRepository> SetupOrganisationQueryRepositoryMock()
        {
            var organisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            organisationQueryRepositoryMock.Setup(r => r.GetByUkPrn(12345678)).ReturnsAsync(GenerateOrganisation(12345678, new Guid("12345678123456781234567812345678")));
            organisationQueryRepositoryMock.Setup(r => r.GetByUkPrn(99999999)).ReturnsAsync(GenerateOrganisation(99999999, new Guid("99999999999999999999999999999999")));
            organisationQueryRepositoryMock.Setup(r => r.GetByUkPrn(55555555)).ReturnsAsync(GenerateOrganisation(55555555, new Guid("55555555555555555555555555555555")));

            return organisationQueryRepositoryMock;
        }

        private static Mock<IStandardService> SetupStandardServiceMock()
        {
            var standardServiceMock = new Mock<IStandardService>();

            standardServiceMock.Setup(c => c.GetEpaoRegisteredStandards("12345678"))
                .ReturnsAsync(new List<EPORegisteredStandards> {
                    GenerateEPORegisteredStandard(1),
                    GenerateEPORegisteredStandard(98),
                    GenerateEPORegisteredStandard(99),
                    GenerateEPORegisteredStandard(101)
                });

            standardServiceMock.Setup(c => c.GetEpaoRegisteredStandards("99999999"))
                .ReturnsAsync(new List<EPORegisteredStandards> {
                    GenerateEPORegisteredStandard(1),
                    GenerateEPORegisteredStandard(99),
                    GenerateEPORegisteredStandard(101)
                });

            standardServiceMock.Setup(c => c.GetEpaoRegisteredStandards("55555555"))
                .ReturnsAsync(new List<EPORegisteredStandards> {
                    GenerateEPORegisteredStandard(55),
                });

            var standard1 = GenerateStandard(1);
            var standard98 = GenerateStandard(98);
            var standard99 = GenerateStandard(99);
            var standard101 = GenerateStandard(101);
            var standard55 = GenerateStandard(55);

            standardServiceMock.Setup(c => c.GetAllStandardVersions())
                .ReturnsAsync(new List<Standard>
                {
                    standard1,
                    standard98,
                    standard99,
                    standard101,
                    standard55
                });

            standardServiceMock.Setup(c => c.GetStandardVersionById("1", It.IsAny<string>())).ReturnsAsync(standard1);
            standardServiceMock.Setup(c => c.GetStandardVersionById("98", It.IsAny<string>())).ReturnsAsync(standard98);
            standardServiceMock.Setup(c => c.GetStandardVersionById("99", It.IsAny<string>())).ReturnsAsync(standard99);
            standardServiceMock.Setup(c => c.GetStandardVersionById("101", It.IsAny<string>())).ReturnsAsync(standard101);
            standardServiceMock.Setup(c => c.GetStandardVersionById("55", It.IsAny<string>())).ReturnsAsync(standard55);

            standardServiceMock.Setup(c => c.GetStandardOptionsByStandardId(standard99.StandardUId)).
                ReturnsAsync(GenerateStandardOptions(new List<string> { "English", "French" }));

            standardServiceMock.Setup(c => c.GetStandardOptionsByStandardId(standard55.StandardUId)).
                ReturnsAsync(GenerateStandardOptions(new List<string> { "English", "French" }));

            standardServiceMock.Setup(c => c.GetStandardOptionsByStandardId(standard98.StandardUId)).
                ReturnsAsync(GenerateStandardOptions(new List<string>()));

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 1))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(1) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 98))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(98) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 99))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(99) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 101))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(101), GenerateEPORegisteredStandardVersion(101, "1.1") });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 1))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(1) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 99))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(99) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 101))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(101) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("55555555", 55))
                .ReturnsAsync(new List<OrganisationStandardVersion> { GenerateEPORegisteredStandardVersion(55), GenerateEPORegisteredStandardVersion(55, "1.1") });

            return standardServiceMock;
        }

        private static Mock<ILearnerRepository> SetupLearnerRepositoryMock()
        {
            var learnerRepositoryMock = new Mock<ILearnerRepository>();

            learnerRepositoryMock.Setup(q => q.Get(1234567890, 1)).ReturnsAsync(GenerateLearner(1234567890, 1, "Test", "12345678", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(1234567890, 98)).ReturnsAsync(GenerateLearner(1234567890, 98, "Test", "12345678", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(1234567890, 99)).ReturnsAsync(GenerateLearner(1234567890, 99, "Test", "12345678", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(1234567890, 101)).ReturnsAsync(GenerateLearner(1234567890, 101, "Test", "12345678", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(9999999999, 1)).ReturnsAsync(GenerateLearner(9999999999, 1, "Test", "99999999", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(9999999999, 99)).ReturnsAsync(GenerateLearner(9999999999, 99, "Test", "99999999", CompletionStatus.Complete, "English"));
            learnerRepositoryMock.Setup(q => q.Get(9999999999, 101)).ReturnsAsync(GenerateLearner(9999999999, 101, "Test", "99999999", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(9876543210, 101)).ReturnsAsync(GenerateLearner(9876543210, 101, "Test", "99999999", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(9876543211, 101)).ReturnsAsync(GenerateLearner(9876543211, 101, "Test", "99999999", CompletionStatus.Complete));

            // Leave this ILR/Learner without a EPA or a Certificate!
            learnerRepositoryMock.Setup(q => q.Get(5555555555, 1)).ReturnsAsync(GenerateLearner(5555555555, 1, "Test", "12345678", CompletionStatus.Complete));

            learnerRepositoryMock.Setup(q => q.Get(1234567891, 1)).ReturnsAsync(GenerateLearner(1234567891, 1, "Test", "12345678", CompletionStatus.Withdrawn));
            learnerRepositoryMock.Setup(q => q.Get(1234567892, 1)).ReturnsAsync(GenerateLearner(1234567892, 1, "Test", "12345678", CompletionStatus.TemporarilyWithdrawn));

            // Learner for testing version and option update validation
            learnerRepositoryMock.Setup(q => q.Get(5555555555, 55)).ReturnsAsync(GenerateLearner(5555555555, 55, "Test", "55555555", CompletionStatus.Complete));
            learnerRepositoryMock.Setup(q => q.Get(5555555556, 55)).ReturnsAsync(GenerateLearner(5555555556, 55, "Test", "55555555", CompletionStatus.Complete, "French"));

            // This is for SV-1260 testing option changed on learner record
            learnerRepositoryMock.Setup(q => q.Get(3333333333, 1)).ReturnsAsync(GenerateLearner(3333333333, 1, "Test", "12345678", CompletionStatus.Complete, "French"));
            // This is fir SV-1260 testing version changed on learner record
            learnerRepositoryMock.Setup(q => q.Get(4343434343, 1)).ReturnsAsync(GenerateLearner(4343434343, 1, "Test", "12345678", CompletionStatus.Complete, version: "1.1"));

            return learnerRepositoryMock;
        }

        private static Certificate GenerateCertificate(long uln, int standardCode, string familyName, string status, Guid organisationId, string version = "1.0")
        {
            var reference = $"{uln}-{standardCode}";

            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
            .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
            .With(i => i.EpaOutcome = EpaOutcome.Pass)
            .Build().ToList();

            var epaDetails = new EpaDetails
            {
                EpaReference = reference,
                LatestEpaDate = epas[0].EpaDate,
                LatestEpaOutcome = epas[0].EpaOutcome,
                Epas = epas
            };

            return Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Status = status)
                .With(i => i.OrganisationId = organisationId)
                .With(i => i.CertificateReference = reference)
                                .With(i => i.CertificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                                .With(cd => cd.LearnerFamilyName = familyName)
                                .With(cd => cd.OverallGrade = CertificateGrade.Pass)
                                .With(cd => cd.EpaDetails = epaDetails)
                                .With(cd => cd.Version = "1.0")
                                .Build()))
                .Build();
        }

        private static Certificate GenerateEpaCertificate(long uln, int standardCode, string familyName, Guid organisationId, bool hasPassedEpa, bool createEpaDetails = true, string overallGrade = null)
        {
            // NOTE: This is to simulate a certificate that has only the EPA part submitted
            var reference = $"{uln}-{standardCode}";

            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
            .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
            .With(i => i.EpaOutcome = hasPassedEpa ? EpaOutcome.Pass : EpaOutcome.Fail)
            .Build().ToList();

            var epaDetails = new EpaDetails();

            if (createEpaDetails)
            {
                epaDetails = new EpaDetails
                {
                    EpaReference = reference,
                    LatestEpaDate = epas[0].EpaDate,
                    LatestEpaOutcome = epas[0].EpaOutcome,
                    Epas = epas
                };
            }

            return Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Status = CertificateStatus.Draft)
                .With(i => i.OrganisationId = organisationId)
                .With(i => i.CertificateReference = $"{uln}-{standardCode}")
                                .With(i => i.CertificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                                .With(cd => cd.LearnerFamilyName = familyName)
                                .With(cd => cd.OverallGrade = overallGrade)
                                .With(cd => cd.AchievementDate = null)
                                .With(cd => cd.EpaDetails = epaDetails)
                                .With(cd => cd.ContactName = null)
                                .With(cd => cd.ContactOrganisation = null)
                                .With(cd => cd.Department = null)
                                .With(cd => cd.ContactAddLine1 = null)
                                .With(cd => cd.ContactAddLine2 = null)
                                .With(cd => cd.ContactAddLine3 = null)
                                .With(cd => cd.ContactAddLine4 = null)
                                .With(cd => cd.ContactPostCode = null)
                                .Build()))
                .Build();
        }

        private static Certificate GeneratePartialCertificate(long uln, int standardCode, string familyName, Guid organisationId, string overallGrade, string status)
        {
            var reference = $"{uln}-{standardCode}";

            var epaDetails = new EpaDetails { Epas = new List<EpaRecord>() };
            if (!string.IsNullOrEmpty(overallGrade))
            {
                var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
                            .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
                            .With(i => i.EpaOutcome = overallGrade == CertificateGrade.Fail ? EpaOutcome.Fail : EpaOutcome.Pass)
                            .Build().ToList();

                epaDetails = new EpaDetails
                {
                    EpaReference = reference,
                    LatestEpaDate = epas[0].EpaDate,
                    LatestEpaOutcome = epas[0].EpaOutcome,
                    Epas = epas
                };
            }


            // NOTE: This is to simulate a certificate that was been partly started via the Web App
            return Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Status = status)
                .With(i => i.OrganisationId = organisationId)
                .With(i => i.CertificateReference = reference)
                                .With(i => i.CertificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                                .With(cd => cd.LearnerFamilyName = familyName)
                                .With(cd => cd.OverallGrade = overallGrade)
                                .With(cd => cd.AchievementDate = null)
                                .With(cd => cd.EpaDetails = epaDetails)
                                .With(cd => cd.ContactName = null)
                                .With(cd => cd.ContactOrganisation = null)
                                .With(cd => cd.Department = null)
                                .With(cd => cd.ContactAddLine1 = null)
                                .With(cd => cd.ContactAddLine2 = null)
                                .With(cd => cd.ContactAddLine3 = null)
                                .With(cd => cd.ContactAddLine4 = null)
                                .With(cd => cd.ContactPostCode = null)
                                .Build()))
                .Build();
        }

        private static Organisation GenerateOrganisation(int ukprn, Guid id)
        {
            return Builder<Organisation>.CreateNew()
                .With(i => i.Id = id)
                .With(i => i.EndPointAssessorOrganisationId = $"{ukprn}")
                .With(i => i.EndPointAssessorUkprn = ukprn)
                .Build();
        }

        private static Standard GenerateStandard(int standardCode)
        {
            return Builder<Standard>.CreateNew()
                .With(i => i.Title = $"{standardCode}")
                .With(i => i.LarsCode = standardCode)
                .With(i => i.IfateReferenceNumber = $"ST{standardCode}")
                .With(i => i.Level = standardCode)
                .With(i => i.Version = "1.0")
                .With(i => i.StandardUId = $"ST{standardCode}_{1.0m}").Build();
        }

        private static OrganisationStandardVersion GenerateEPORegisteredStandardVersion(int standardCode, string standardVersion = "1.0")
        {
            return Builder<OrganisationStandardVersion>.CreateNew()
                .With(i => i.Title = $"{standardCode}")
                .With(i => i.LarsCode = standardCode)
                .With(i => i.Level = standardCode)
                .With(i => i.Version = standardVersion)
                .Build();
        }

        private static EPORegisteredStandards GenerateEPORegisteredStandard(int standardCode)
        {
            return Builder<EPORegisteredStandards>.CreateNew()
                .With(i => i.StandardName = $"{standardCode}")
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Level = standardCode)
                .Build();
        }

        private static Learner GenerateLearner(long uln, int standardCode, string familyName, string epaOrgId, CompletionStatus completionStatus, string courseOption = null, bool versionConfirmed = true, string version = "1.0")
        {
            return Builder<Learner>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StdCode = standardCode)
                .With(i => i.FamilyName = familyName)
                .With(i => i.EpaOrgId = epaOrgId)
                .With(i => i.CompletionStatus = (int)completionStatus)
                .With(i => i.VersionConfirmed = versionConfirmed)
                .With(i => i.Version = version)
                .With(i => i.CourseOption = courseOption)
                .Build();
        }

        private static StandardOptions GenerateStandardOptions(IEnumerable<string> options)
        {
            return Builder<StandardOptions>.CreateNew()
                .With(o => o.CourseOption = options)
                .Build();
        }
    }
}
