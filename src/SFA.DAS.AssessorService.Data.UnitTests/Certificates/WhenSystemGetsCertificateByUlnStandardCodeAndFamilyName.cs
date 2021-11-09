using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByUlnStandardCodeAndFamilyName
    {

        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<AssessorDbContext> _mockDbContext;

        private CertificateData _certificateData;
        private Certificate _certificate;
        
        private CertificateRepository _certificateRepository;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var mockSet = CreateCertificateMockDbSet();
            _mockDbContext = CreateMockDbContext(mockSet);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);
        }

        [Test, Ignore("Temporarily ignore during .Net Core 3.1 upgrade")]
        public async Task Then_ReturnResult()
        {
            var result = await _certificateRepository.GetCertificate(_certificate.Uln, _certificate.StandardCode, _certificateData.LearnerFamilyName);

            result.Should().BeEquivalentTo(_certificate);
        }

        [Test, Ignore("Temporarily ignore during .Net Core 3.1 upgrade")]
        public async Task And_NameMatchesWhenIgnoringCase_Then_ReturnResult()
        {
            var result = await _certificateRepository.GetCertificate(_certificate.Uln, _certificate.StandardCode, _certificateData.LearnerFamilyName.ToUpper());

            result.Should().BeEquivalentTo(_certificate);
        }

        [Test, Ignore("Temporarily ignore during .Net Core 3.1 upgrade")]
        public async Task And_FamilyNameIsNotCorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(_certificate.Uln, _certificate.StandardCode, "IncorrectName");

            result.Should().BeNull();
        }

        [Test, Ignore("Temporarily ignore during .Net Core 3.1 upgrade")]
        public async Task And_UlnIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(9999999999, _certificate.StandardCode, _certificateData.LearnerFamilyName);

            result.Should().BeNull();
        }

        [Test, Ignore("Temporarily ignore during .Net Core 3.1 upgrade")]
        public async Task And_StandardCodeIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(_certificate.Uln, 2, _certificateData.LearnerFamilyName);

            result.Should().BeNull();
        }

        private Mock<DbSet<Certificate>> CreateCertificateMockDbSet()
        {
            _certificateData = Builder<CertificateData>.CreateNew().Build();

            var certificateDataJson = JsonConvert.SerializeObject(_certificateData);

            _certificate = Builder<Certificate>.CreateNew()
                .With(x => x.CertificateData = certificateDataJson).Build();

            var certificates = Builder<Certificate>.CreateListOfSize(9)
                .TheFirst(9)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.StandardCode = 100)
                .With(x => x.IsPrivatelyFunded = true)
                .Build()
                .Append(_certificate)
                .AsQueryable();

            var mockSet = new Mock<DbSet<Certificate>>();

            //mockSet.As<IAsyncEnumerable<Certificate>>()
                // .Setup(m => m.GetEnumerator())// @ToDo: .Net Core 3.1 upgrade - uncomment and fix this
                //.Returns(new TestAsyncEnumerator<Certificate>(certificates.GetEnumerator()));

            mockSet.As<IQueryable<Certificate>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Certificate>(certificates.Provider));

            mockSet.As<IQueryable<Certificate>>().Setup(m => m.Expression).Returns(certificates.Expression);
            mockSet.As<IQueryable<Certificate>>().Setup(m => m.ElementType).Returns(certificates.ElementType);
            mockSet.As<IQueryable<Certificate>>().Setup(m => m.GetEnumerator()).Returns(() => certificates.GetEnumerator());

            return mockSet;
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Certificate>> certificateMockDbSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.Certificates).Returns(certificateMockDbSet.Object);
            return mockDbContext;
        }
    }
}
