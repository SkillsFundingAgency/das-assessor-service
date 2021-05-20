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
namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi
{
    public class ExternalApiValidatorsTestBase
    {
        public Mock<ICertificateRepository> CertificateRepositoryMock { get; }
        public Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock { get; }
        public Mock<IIlrRepository> IlrRepositoryMock { get; }
        public Mock<IStandardService> StandardServiceMock { get; }

        public ExternalApiValidatorsTestBase()
        {
            CertificateRepositoryMock = SetupCertificateRepositoryMock();
            OrganisationQueryRepositoryMock = SetupOrganisationQueryRepositoryMock();
            IlrRepositoryMock = SetupIlrRepositoryMock();
            StandardServiceMock = SetupStandardServiceMock();
        }

        private static Mock<ICertificateRepository> SetupCertificateRepositoryMock()
        {
            var certificateRepositoryMock = new Mock<ICertificateRepository>();

            // Having a range of certificates for 2 EPAO's (with a shared standard) allows us to test the suite of validation ruleS
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 1)).ReturnsAsync(GenerateCertificate(1234567890, 1, "test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 98)).ReturnsAsync(GenerateCertificate(1234567890, 98, "test", CertificateStatus.Deleted, new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 1)).ReturnsAsync(GenerateCertificate(9999999999, 1, "test", CertificateStatus.Printed, new Guid("99999999999999999999999999999999")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 99)).ReturnsAsync(GenerateCertificate(1234567890, 99, "Test", CertificateStatus.Draft, new Guid("12345678123456781234567812345678")));

            // This is simulating a Certificate that started it's life on the Web App, but was never submitted
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 99)).ReturnsAsync(GeneratePartialCertificate(1234567890, 99, "test", new Guid("12345678123456781234567812345678"), null));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 99)).ReturnsAsync(GeneratePartialCertificate(9999999999, 99, "test", new Guid("99999999999999999999999999999999"), CertificateGrade.Fail));

            // These allow us to test EPAs, which is the initial stage of a Certificate
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 101)).ReturnsAsync(GenerateEpaCertificate(1234567890, 101, "test", new Guid("12345678123456781234567812345678"), true));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 101)).ReturnsAsync(GenerateEpaCertificate(9999999999, 101, "test", new Guid("99999999999999999999999999999999"), false));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9876543210, 101)).ReturnsAsync(GenerateEpaCertificate(9876543210, 101, "test", new Guid("99999999999999999999999999999999"), false));


            return certificateRepositoryMock;
        }

        private static Mock<IOrganisationQueryRepository> SetupOrganisationQueryRepositoryMock()
        {
            var organisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            organisationQueryRepositoryMock.Setup(r => r.GetByUkPrn(12345678)).ReturnsAsync(GenerateOrganisation(12345678, new Guid("12345678123456781234567812345678")));
            organisationQueryRepositoryMock.Setup(r => r.GetByUkPrn(99999999)).ReturnsAsync(GenerateOrganisation(99999999, new Guid("99999999999999999999999999999999")));

            return organisationQueryRepositoryMock;
        }

        private static Mock<IStandardService> SetupStandardServiceMock()
        {
            var standardServiceMock = new Mock<IStandardService>();

            // Original Setup as the test class is shared by multiple validators
            // Can be removed at end of external api development work
            // Begin

            var standardCollation1 = GenerateStandardCollation(1, new List<string>());
            var standardCollation98 = GenerateStandardCollation(98, new List<string>());
            var standardCollation99 = GenerateStandardCollation(99,
                new List<string>
                {
                    "English",
                    "French"
                });
            var standardCollation101 = GenerateStandardCollation(101, new List<string>());

            standardServiceMock.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<StandardCollation>
                {
                        standardCollation1,
                        standardCollation98,
                        standardCollation99,
                        standardCollation101
                });

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

            // End Original Setup

            var standard1 = GenerateStandard(1);
            var standard98 = GenerateStandard(98);
            var standard99 = GenerateStandard(99); // Missing Options From Above
            var standard101 = GenerateStandard(101);

            standardServiceMock.Setup(c => c.GetStandard(1)).ReturnsAsync(standardCollation1);
            standardServiceMock.Setup(c => c.GetStandard(98)).ReturnsAsync(standardCollation98);
            standardServiceMock.Setup(c => c.GetStandard(99)).ReturnsAsync(standardCollation99);
            standardServiceMock.Setup(c => c.GetStandard(101)).ReturnsAsync(standardCollation101);

            standardServiceMock.Setup(c => c.GetAllStandardVersions())
                .ReturnsAsync(new List<Standard>
                {
                    standard1,
                    standard98,
                    standard99,
                    standard101
                });

            standardServiceMock.Setup(c => c.GetStandardVersionById("1", null)).ReturnsAsync(standard1);
            standardServiceMock.Setup(c => c.GetStandardVersionById("98", null)).ReturnsAsync(standard98);
            standardServiceMock.Setup(c => c.GetStandardVersionById("99", null)).ReturnsAsync(standard99);
            standardServiceMock.Setup(c => c.GetStandardVersionById("101", null)).ReturnsAsync(standard101);

            standardServiceMock.Setup(c => c.GetStandardOptionsByStandardId(standard99.StandardUId)).
                ReturnsAsync(GenerateStandardOptions(new List<string> { "English", "French" }));

            standardServiceMock.Setup(c => c.GetStandardOptionsByStandardId(standard98.StandardUId)).
                ReturnsAsync(GenerateStandardOptions(new List<string>()));

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 1))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(1) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 98))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(98) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 99))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(99) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("12345678", 101))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(101) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 1))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(1) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 99))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(99) });

            standardServiceMock.Setup(c => c.GetEPAORegisteredStandardVersions("99999999", 101))
                .ReturnsAsync(new List<StandardVersion> { GenerateEPORegisteredStandardVersion(101) });

            return standardServiceMock;
        }

        private static Mock<IIlrRepository> SetupIlrRepositoryMock()
        {
            var ilrRepositoryMock = new Mock<IIlrRepository>();

            ilrRepositoryMock.Setup(q => q.Get(1234567890, 1)).ReturnsAsync(GenerateIlr(1234567890, 1, "Test", "12345678", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 98)).ReturnsAsync(GenerateIlr(1234567890, 98, "Test", "12345678", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 99)).ReturnsAsync(GenerateIlr(1234567890, 99, "Test", "12345678", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 101)).ReturnsAsync(GenerateIlr(1234567890, 101, "Test", "12345678", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 1)).ReturnsAsync(GenerateIlr(9999999999, 1, "Test", "99999999", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 99)).ReturnsAsync(GenerateIlr(9999999999, 99, "Test", "99999999", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 101)).ReturnsAsync(GenerateIlr(9999999999, 101, "Test", "99999999", CompletionStatus.Complete));
            ilrRepositoryMock.Setup(q => q.Get(9876543210, 101)).ReturnsAsync(GenerateIlr(9876543210, 101, "Test", "99999999", CompletionStatus.Complete));

            // Leave this ILR without a EPA or a Certificate!
            ilrRepositoryMock.Setup(q => q.Get(5555555555, 1)).ReturnsAsync(GenerateIlr(5555555555, 1, "Test", "12345678", CompletionStatus.Complete));

            ilrRepositoryMock.Setup(q => q.Get(1234567891, 1)).ReturnsAsync(GenerateIlr(1234567891, 1, "Test", "12345678", CompletionStatus.Withdrawn));
            ilrRepositoryMock.Setup(q => q.Get(1234567892, 1)).ReturnsAsync(GenerateIlr(1234567892, 1, "Test", "12345678", CompletionStatus.TemporarilyWithdrawn));

            return ilrRepositoryMock;
        }

        private static Certificate GenerateCertificate(long uln, int standardCode, string familyName, string status, Guid organisationId)
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
                                .Build()))
                .Build();
        }

        private static Certificate GenerateEpaCertificate(long uln, int standardCode, string familyName, Guid organisationId, bool hasPassedEpa, bool createEpaDetails = true)
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
                                .With(cd => cd.OverallGrade = null)
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

        private static Certificate GeneratePartialCertificate(long uln, int standardCode, string familyName, Guid organisationId, string overallGrade)
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
                .With(i => i.Status = CertificateStatus.Draft)
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
                .With(i => i.Version = 1.0m)
                .With(i => i.StandardUId = $"ST{standardCode}_{1.0m}").Build();
        }

        private static StandardCollation GenerateStandardCollation(int standardCode, List<string> options)
        {
            return Builder<StandardCollation>.CreateNew()
                .With(i => i.Title = $"{standardCode}")
                .With(i => i.StandardId = standardCode)
                .With(i => i.ReferenceNumber = $"{standardCode}")
                .With(i => i.StandardData = new StandardData() { Level = standardCode })
                .With(i => i.Options = options).Build();
        }

        private static StandardVersion GenerateEPORegisteredStandardVersion(int standardCode)
        {
            return Builder<StandardVersion>.CreateNew()
                .With(i => i.Title = $"{standardCode}")
                .With(i => i.LarsCode = standardCode)
                .With(i => i.Level = standardCode)
                .With(i => i.Version = "1.0")
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

        private static Ilr GenerateIlr(long uln, int standardCode, string familyName, string epaOrgId, CompletionStatus completionStatus)
        {
            return Builder<Ilr>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StdCode = standardCode)
                .With(i => i.FamilyName = familyName)
                .With(i => i.EpaOrgId = epaOrgId)
                .With(i => i.CompletionStatus = (int)completionStatus)
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
