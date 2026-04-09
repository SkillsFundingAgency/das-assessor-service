using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    [TestFixture]
    public class SearchCertificatesHandlerTests
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<ILogger<SearchCertificatesHandler>> _logger;
        private SearchCertificatesHandler _handler;

        [SetUp]
        public void Setup()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _logger = new Mock<ILogger<SearchCertificatesHandler>>();
            _handler = new SearchCertificatesHandler(_certificateRepository.Object, _logger.Object);
        }

        [Test]
        public async Task Handle_ReturnsResultsFromRepository()
        {
            var dob = new DateTime(1990, 1, 1);
            var name = "Smith";
            var exclude = new long[] { 1111111111 };

            var repoResults = new List<SearchCertificatesResponse>
            {
                new SearchCertificatesResponse { Uln = 2222222222, CertificateType = "Standard", CourseCode = "100", CourseName = "Test", CourseLevel = "3", DateAwarded = DateTime.Today, ProviderName = "Prov" }
            };

            _certificateRepository.Setup(r => r.SearchByDobAndFamilyName(dob, name, It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(repoResults);

            var request = new SearchCertificatesRequest { DateOfBirth = dob, Name = name, Exclude = exclude };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2222222222, result[0].Uln);
        }
    }
}
