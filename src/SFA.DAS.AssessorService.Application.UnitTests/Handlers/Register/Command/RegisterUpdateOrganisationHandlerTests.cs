using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
    [TestFixture]
    public class RegisterUpdateOrganisationHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private UpdateEpaOrganisationHandler _updateEpaOrganisationHandler;
        private string _returnedOrganisationId;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<ILogger<UpdateEpaOrganisationHandler>> _logger;
        private UpdateEpaOrganisationRequest _requestNoIssues;
        private EpaOrganisation _expectedOrganisationNoIssues;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _logger = new Mock<ILogger<UpdateEpaOrganisationHandler>>();
            _organisationId = "EPA999";

            _requestNoIssues = BuildRequest("name 1", _organisationId, 123321);
            _expectedOrganisationNoIssues = BuildOrganisation(_requestNoIssues);

            _registerRepository.Setup(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()))
                .Returns(Task.FromResult(_expectedOrganisationNoIssues.OrganisationId));

            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckOrganisationName(_requestNoIssues.Name)).Returns(string.Empty);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationUkprnExistsForOtherOrganisations(_requestNoIssues.Ukprn,_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _validator.Setup(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId)).Returns(string.Empty);
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);
            
            _updateEpaOrganisationHandler = new UpdateEpaOrganisationHandler(_registerRepository.Object, _validator.Object, _logger.Object, _cleanserService.Object);
        }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var res = _updateEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()));
        }

        [Test]
        public void CheckAllValidatorsAreCalledWhenHandlerInvoked()
        {
            var res = _updateEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckOrganisationName(_requestNoIssues.Name));
            _validator.Verify(v => v.CheckIfOrganisationUkprnExistsForOtherOrganisations(_requestNoIssues.Ukprn, _requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckIfOrganisationNotFound(_requestNoIssues.OrganisationId));
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(_requestNoIssues.OrganisationTypeId));
        }

        [Test]
        public void GetOrganisationDetailsWhenOrganisationUpdated()
        {
            _returnedOrganisationId = _updateEpaOrganisationHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedOrganisationId.Should().BeEquivalentTo(_expectedOrganisationNoIssues.OrganisationId);
        }

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIsNotFound()
        {
            const string errorMessage = "organisation not found";
            var requestOrganisationNotFound = BuildRequest("name", "EPS999", 123321);
            _validator.Setup(v => v.CheckIfOrganisationNotFound(requestOrganisationNotFound.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<NotFound>(() => _updateEpaOrganisationHandler.Handle(requestOrganisationNotFound, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationNotFound(requestOrganisationNotFound.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenOrganisationIdIssueValidationOccurs()
        {
            const string errorMessage = "id issue";
            var requestNoOrgId = BuildRequest("name", "EPS999", 123321);
            _validator.Setup(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationHandler.Handle(requestNoOrgId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationIdIsPresentAndValid(requestNoOrgId.OrganisationId));
        }

        [Test]
        public void GetBadRequestExceptionWhenNoNameValidationOccurs()
        {
            const string errorMessage = "No Name issue";
            var requestNoName = BuildRequest("", _organisationId, 123321);
            _validator.Setup(v => v.CheckOrganisationName(requestNoName.Name)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationHandler.Handle(requestNoName, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationName(requestNoName.Name));
        }

        [Test]
        public void GetBadRequestExceptionWhenBadOrganisationTypeValidationOccurs()
        {
            const string errorMessage = "organisation type id doesn't exist";
            var requestInvalidOrgTypeId = BuildRequest("name", "EPA888", 123321);
            _validator.Setup(v => v.CheckOrganisationTypeIsNullOrExists(requestInvalidOrgTypeId.OrganisationTypeId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationHandler.Handle(requestInvalidOrgTypeId, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckOrganisationTypeIsNullOrExists(requestInvalidOrgTypeId.OrganisationTypeId));
        }

        [Test]
        public void GetBadRequestExceptionWhenUkprnAlreadyExistsForOrtherOrganisationValidationOccurs()
        {
            const string errorMessage = "ukprn already exists iwth other organisation";
            var requestUkprnExistsElsewhere = BuildRequest("name", "EPA888", 123321);
            _validator.Setup(v => v.CheckIfOrganisationUkprnExistsForOtherOrganisations(requestUkprnExistsElsewhere.Ukprn, requestUkprnExistsElsewhere.OrganisationId)).Returns(errorMessage);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationHandler.Handle(requestUkprnExistsElsewhere, new CancellationToken()));
            Assert.AreEqual(errorMessage, ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisation(It.IsAny<EpaOrganisation>()), Times.Never);
            _validator.Verify(v => v.CheckIfOrganisationUkprnExistsForOtherOrganisations(requestUkprnExistsElsewhere.Ukprn, requestUkprnExistsElsewhere.OrganisationId));
        }

        private UpdateEpaOrganisationRequest BuildRequest(string name, string organisationId, long? ukprn)
        {
            return new UpdateEpaOrganisationRequest
            {
                Name = name,
                OrganisationId = organisationId,
                Ukprn = ukprn,
                OrganisationTypeId = 5,
                LegalName = "legal name 1",
                WebsiteLink = "website link 1",
                Address1 = "address 1",
                Address2 = "address 2",
                Address3 = "address 3",
                Address4 = "address 4",
                Postcode = "postcode"
            };
        }

        private EpaOrganisation BuildOrganisation(UpdateEpaOrganisationRequest request)
        {
            return new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = request.Name,
                OrganisationId = request.OrganisationId,
                Ukprn = request.Ukprn,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = request.OrganisationTypeId,
                OrganisationData = new OrganisationData
                {
                    LegalName = request.LegalName,
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    Postcode = request.Postcode
                }
            };
        }
    }
}
