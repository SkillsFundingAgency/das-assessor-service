using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificateAddressHandler
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;      

        private List<CertificateAddress> _result;


        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();                                 
            var cetificateAddresses = Builder<CertificateAddress>.CreateListOfSize(10).Build()
                .ToList();         

            _certificateRepositoryMock.Setup(q => q.GetPreviousAddresses("TestUser"))
                .Returns(Task.FromResult(cetificateAddresses));


            var getCertificatesHandler =
                new GetPreviousAddressesHandler(_certificateRepositoryMock.Object);

            _result = getCertificatesHandler.Handle(new GetPreviousAddressesRequest { Username = "TestUser"}, new CancellationToken())
                .Result;
        }

        [Test]
        public void then_certificates_are_returned()
        {
            _result.Count().Should().BeGreaterOrEqualTo(10);
        }
    }
}
