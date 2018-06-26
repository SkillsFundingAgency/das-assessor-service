using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.Query
{
    public class WhenGetCertificateAddressHandler
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<IOrganisationQueryRepository> _organisationQueryRepositoryMock;

        private List<CertificateAddressResponse> _result;


        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();            
            var organisation = Builder<Organisation>
                .CreateNew()                
                .Build();

            var contact = Builder<Contact>.CreateNew()
                .With(q => q.OrganisationId = organisation.Id)
                .With(q => q.Organisation = organisation)
                .Build();

            var cetificateAddresses = Builder<CertificateAddress>.CreateListOfSize(10).Build()
                .ToList();

            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _certificateRepositoryMock = new Mock<ICertificateRepository>();        
            _organisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            _contactQueryRepositoryMock.Setup(r => r.GetContact(It.IsAny<string>())).Returns(Task.FromResult(contact));
            _organisationQueryRepositoryMock.Setup(q => q.Get(contact.EndPointAssessorOrganisationId))
                .Returns(Task.FromResult(organisation));

            _certificateRepositoryMock.Setup(q => q.GetPreviousAddresses(organisation.Id))
                .Returns(Task.FromResult(cetificateAddresses));


            var getCertificatesHandler =
                new GetPreviousAddressesHandler(
                    _contactQueryRepositoryMock.Object,
                    _certificateRepositoryMock.Object,
                    _organisationQueryRepositoryMock.Object
                    );

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
