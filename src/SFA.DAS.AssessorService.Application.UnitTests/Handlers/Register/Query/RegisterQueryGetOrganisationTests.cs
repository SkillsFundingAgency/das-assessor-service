using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetOrganisationTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetAssessmentOrganisationHandler GetAssessmentOrganisationDetailsHandler;
        protected Mock<ILogger<GetAssessmentOrganisationHandler>> Logger;
        private AssessmentOrganisationDetails _initialOrganisationDetails; 
        private GetAssessmentOrganisationRequest _requestDetails;
        private AssessmentOrganisationContact _expectedContact;
        private List<AssessmentOrganisationContact> _expectedContacts;
        private AssessmentOrganisationAddress _expectedAddress;
        private List<AssessmentOrganisationAddress> _expectedAddresses;
        private const string OrganisationId = "DEF345";

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
          
            Logger = new Mock<ILogger<GetAssessmentOrganisationHandler>>();

            _requestDetails = new GetAssessmentOrganisationRequest {OrganisationId = OrganisationId};

            _expectedAddress = new AssessmentOrganisationAddress
            {
                Primary = "Address line 1",
                Secondary = "Address line 2",
                Street = "Street line",
                Town = "Town line",
                Postcode = "CV2 4EE"
            };

            _expectedAddresses = new List<AssessmentOrganisationAddress>
            {
                _expectedAddress
            };      

            _expectedContact = new AssessmentOrganisationContact
            {
                OrganisationId = OrganisationId,
                PhoneNumber = "123 456",
                Email = "testy@mctestface.com"
            };

            _expectedContacts = new List<AssessmentOrganisationContact>
            {
                _expectedContact
            };

            _initialOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = null,
                Phone = null,
                Address = null
            };
            
            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisation(OrganisationId))
                .Returns(Task.FromResult(_initialOrganisationDetails));

            GetAssessmentOrganisationDetailsHandler = new GetAssessmentOrganisationHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisation(OrganisationId));
        }

        [Test]
        public void GetOrganisationDetailsExpectedForOneAddressAndOneContact()
        {
            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(_expectedAddresses.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(_expectedContact));

            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = _expectedContact.Email,
                Phone = _expectedContact.PhoneNumber,
                Address = _expectedAddress
            };
            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }

        [Test]
        public void GetOrganisationDetailsExpectedForNoAddressAndOneContact()
        {
            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(new List<AssessmentOrganisationAddress>().AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(_expectedContact));

            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = _expectedContact.Email,
                Phone = _expectedContact.PhoneNumber,
                Address = null
            };

            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }

        [Test]
        public void GetOrganisationDetailsExpectedForOneAddressAndNoContact()
        {
            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(_expectedAddresses.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(new AssessmentOrganisationContact()));

            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = null,
                Phone = null,
                Address = _expectedAddress
            };
            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }


        [Test]
        public void GetOrganisationDetailsExpectedForThreeAddressesAndOneContact()
        {
            var expectedAddresses = new List<AssessmentOrganisationAddress>
            {
                new AssessmentOrganisationAddress { Primary = "Addr 1", Secondary = "Addr 2", Street = "Street line", Town = "Town line", Postcode = "CV2 4EE"},
                new AssessmentOrganisationAddress { Primary = "Addr 10", Secondary = "Addr 20", Street = "Street line 40", Town = "Town line 7", Postcode = "CV2 4FF"},
                new AssessmentOrganisationAddress { Primary = "Addr 100", Secondary = "Addr 200", Street = "Street line 4", Town = "Town line 6", Postcode = "CV2 4AA"},
            };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(expectedAddresses.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(_expectedContact));

            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = _expectedContact.Email,
                Phone = _expectedContact.PhoneNumber,
                Address = expectedAddresses.First()
            };

            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }

        [Test]
        public void GetOrganisationDetailExpectedForOneAddressAndThreeContactsWithoutPrimaryContact()
        {

            var expectedContacts = new List<AssessmentOrganisationContact>
            {
                new AssessmentOrganisationContact { OrganisationId = OrganisationId, PhoneNumber = "123 456", Email = "testy@mctestface.com"},
                new AssessmentOrganisationContact { OrganisationId = OrganisationId, PhoneNumber = "654 321", Email = "tester@mctestface.com"},
                new AssessmentOrganisationContact { OrganisationId = OrganisationId, PhoneNumber = "111222", Email = "testmark@mctestface.com"}
            };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(_expectedAddresses.AsEnumerable()));

            var expectedContact = expectedContacts.First();

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(expectedContact));
          
            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = expectedContact.Email,
                Phone = expectedContact.PhoneNumber,
                Address = _expectedAddress
            };
            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }


        [Test]
        public void GetOrganisationDetailsReturnedForOneAddressAndThreeContactsWithOnePrimaryContact()
        {

            var contactWithPrimaryContactSet = new AssessmentOrganisationContact
            {
                OrganisationId = OrganisationId,
                PhoneNumber = "654 321",
                Email = "tester@mctestface.com",
                IsPrimaryContact = true
            };


            var expectedContacts = new List<AssessmentOrganisationContact>
            {
                contactWithPrimaryContactSet,
                new AssessmentOrganisationContact { OrganisationId = OrganisationId, PhoneNumber = "123 456", Email = "testy@mctestface.com"},
                new AssessmentOrganisationContact { OrganisationId = OrganisationId, PhoneNumber = "111222", Email = "testmark@mctestface.com"}
            };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationAddresses(OrganisationId))
                .Returns(Task.FromResult(_expectedAddresses.AsEnumerable()));

            RegisterQueryRepository.Setup(r => r.GetPrimaryOrFirstContact(OrganisationId))
                .Returns(Task.FromResult(expectedContacts.First()));

            var expectedContact = contactWithPrimaryContactSet;

            var expectedOrganisationDetails = new AssessmentOrganisationDetails
            {
                Id = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                Email = expectedContact.Email,
                Phone = expectedContact.PhoneNumber,
                Address = _expectedAddress
            };
            var organisation = GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Result;
            organisation.Should().BeEquivalentTo(expectedOrganisationDetails);
        }
    }
}
