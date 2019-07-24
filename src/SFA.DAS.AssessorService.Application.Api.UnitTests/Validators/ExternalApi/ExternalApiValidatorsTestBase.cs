using FizzWare.NBuilder;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
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
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 1)).ReturnsAsync(GenerateCertificate(1234567890, 1, "test", "Draft", new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 98)).ReturnsAsync(GenerateCertificate(1234567890, 98, "test", "Deleted", new Guid("12345678123456781234567812345678")));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 1)).ReturnsAsync(GenerateCertificate(9999999999, 1, "test", "Printed", new Guid("99999999999999999999999999999999")));

            // This is simulating a Certificate that started it's life on the Web App, but was never submitted
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 99)).ReturnsAsync(GeneratePartialCertificate(9999999999, 99, "test", new Guid("99999999999999999999999999999999")));
            
            // These allow use to test EPAs, which is the initial stage of a Certificate
            certificateRepositoryMock.Setup(q => q.GetCertificate(1234567890, 101)).ReturnsAsync(GenerateEpaCertificate(1234567890, 101, "test", new Guid("12345678123456781234567812345678"), true));
            certificateRepositoryMock.Setup(q => q.GetCertificate(9999999999, 101)).ReturnsAsync(GenerateEpaCertificate(9999999999, 101, "test", new Guid("99999999999999999999999999999999"), false));

            certificateRepositoryMock.Setup(q => q.GetOptions(1)).ReturnsAsync(new List<Option>());
            certificateRepositoryMock.Setup(q => q.GetOptions(98)).ReturnsAsync(new List<Option>());
            certificateRepositoryMock.Setup(q => q.GetOptions(101)).ReturnsAsync(new List<Option>());

            certificateRepositoryMock.Setup(q => q.GetOptions(99))
                .ReturnsAsync(new List<Option>
                {
                   GenerateOption(99, "English"),
                   GenerateOption(99, "French")
                });

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
            standardServiceMock.Setup(c => c.GetAllStandards())
                .ReturnsAsync(new List<StandardCollation>
                {
                    GenerateStandard(1),
                    GenerateStandard(98),
                    GenerateStandard(99),
                    GenerateStandard(101)
                });

            standardServiceMock.Setup(c => c.GetStandard(1)).ReturnsAsync(GenerateStandard(1));
            standardServiceMock.Setup(c => c.GetStandard(98)).ReturnsAsync(GenerateStandard(98));
            standardServiceMock.Setup(c => c.GetStandard(99)).ReturnsAsync(GenerateStandard(99));
            standardServiceMock.Setup(c => c.GetStandard(101)).ReturnsAsync(GenerateStandard(101));

            standardServiceMock.Setup(c => c.GetEpaoRegisteredStandards("12345678"))
                .ReturnsAsync(new List<EPORegisteredStandards>
                {
                    GenerateEPORegisteredStandard(1),
                    GenerateEPORegisteredStandard(98),
                    GenerateEPORegisteredStandard(99),
                    GenerateEPORegisteredStandard(101)
                });

            standardServiceMock.Setup(c => c.GetEpaoRegisteredStandards("99999999"))
                .ReturnsAsync(new List<EPORegisteredStandards>
                {
                    GenerateEPORegisteredStandard(1),
                    GenerateEPORegisteredStandard(99),
                    GenerateEPORegisteredStandard(101)
                });

            return standardServiceMock;
        }

        private static Mock<IIlrRepository> SetupIlrRepositoryMock()
        {
            var ilrRepositoryMock = new Mock<IIlrRepository>();

            ilrRepositoryMock.Setup(q => q.Get(1234567890, 1)).ReturnsAsync(GenerateIlr(1234567890, 1, "Test", "12345678"));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 98)).ReturnsAsync(GenerateIlr(1234567890, 98, "Test", "12345678"));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 99)).ReturnsAsync(GenerateIlr(1234567890, 99, "Test", "12345678"));
            ilrRepositoryMock.Setup(q => q.Get(1234567890, 101)).ReturnsAsync(GenerateIlr(1234567890, 101, "Test", "12345678"));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 1)).ReturnsAsync(GenerateIlr(9999999999, 1, "Test", "99999999"));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 99)).ReturnsAsync(GenerateIlr(9999999999, 99, "Test", "99999999"));
            ilrRepositoryMock.Setup(q => q.Get(9999999999, 101)).ReturnsAsync(GenerateIlr(9999999999, 101, "Test", "99999999"));

            return ilrRepositoryMock;
        }

        private static Certificate GenerateCertificate(long uln, int standardCode, string familyName, string status, Guid organisationId)
        {
            var reference = $"{uln}-{standardCode}";

            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
            .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
            .With(i => i.EpaOutcome = "pass")
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
                                .With(cd => cd.OverallGrade = "Pass")
                                .With(cd => cd.EpaDetails = epaDetails)
                                .Build()))
                .Build();
        }

        private static Certificate GenerateEpaCertificate(long uln, int standardCode, string familyName, Guid organisationId, bool hasPassedEpa)
        {
            // NOTE: This is to simulate a certificate that has only the EPA part submitted
            var reference = $"{uln}-{standardCode}";

            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
            .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
            .With(i => i.EpaOutcome = hasPassedEpa ? "pass" : "fail")
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
                .With(i => i.Status = "Draft")
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

        private static Certificate GeneratePartialCertificate(long uln, int standardCode, string familyName, Guid organisationId)
        {
            // NOTE: This is to simulate a certificate that was been partly started via the Web App
            return Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Status = "Draft")
                .With(i => i.OrganisationId = organisationId)
                .With(i => i.CertificateReference = $"{uln}-{standardCode}")
                                .With(i => i.CertificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                                .With(cd => cd.LearnerFamilyName = familyName)
                                .With(cd => cd.OverallGrade = null)
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

        private static StandardCollation GenerateStandard(int standardCode)
        {
            return Builder<StandardCollation>.CreateNew()
                .With(i => i.Title = $"{standardCode}")
                .With(i => i.StandardId = standardCode)
                .With(i => i.ReferenceNumber = $"{standardCode}")
                .With(i => i.StandardData = new StandardData() { Level = standardCode }).Build();
        }

        private static EPORegisteredStandards GenerateEPORegisteredStandard(int standardCode)
        {
            return Builder<EPORegisteredStandards>.CreateNew()
                .With(i => i.StandardName = $"{standardCode}")
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Level = standardCode)
                .Build();
        }

        private static Ilr GenerateIlr(long uln, int standardCode, string familyName, string epaOrgId)
        {
            return Builder<Ilr>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StdCode = standardCode)
                .With(i => i.FamilyName = familyName)
                .With(i => i.EpaOrgId = epaOrgId)
                .Build();
        }

        private static Option GenerateOption(int standardCode, string optionName)
        {
            return Builder<Option>.CreateNew()
                .With(o => o.StdCode = standardCode)
                .With(o => o.OptionName = optionName)
                .Build();
        }
    }
}
