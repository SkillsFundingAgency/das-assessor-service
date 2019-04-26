using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
    [TestFixture]
    public class RegisterCreateOrganisationContactHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private CreateEpaOrganisationContactHandler _createEpaOrganisationContactHandler;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ILogger<CreateEpaOrganisationContactHandler>> _logger;
        private Mock<IEpaOrganisationIdGenerator> _idGenerator;
        private CreateEpaOrganisationContactRequest _requestNoIssues;
        private string _organisationId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private EpaContact _expectedOrganisationContactNoIssues;
        private string _requestNoIssuesUserName;

        private string _returnedUserName;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<CreateEpaOrganisationContactHandler>>();
            _idGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _organisationId = "EPA999";
            _firstName = "Testy";
            _lastName = "McTestFace";

            _email = "testy@testface.com";
            _phoneNumber = "555 5555";
            _requestNoIssuesUserName = "testy@testface.com";
            _requestNoIssues = BuildRequest(_organisationId,_firstName,_lastName,_email,_phoneNumber);
            _expectedOrganisationContactNoIssues = BuildOrganisationContact(_requestNoIssues, _requestNoIssuesUserName);

            _registerRepository.Setup(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()))
                .Returns(Task.FromResult(_expectedOrganisationContactNoIssues.Username));

            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _validator.Setup(v => v.ValidatorCreateEpaOrganisationContactRequest(_requestNoIssues))
                .Returns(new ValidationResponse());
            _idGenerator.Setup(i => i.GetNextContactUsername()).Returns(_requestNoIssuesUserName);

            _createEpaOrganisationContactHandler = new CreateEpaOrganisationContactHandler(_registerRepository.Object, _validator.Object, _cleanserService.Object, _logger.Object, _idGenerator.Object);
        }

        [Test]
        public void CreateOrganisationContactDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var res = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()));
        }

        [Test]
        public void CheckMainValidatorIsCalledWhenHandlerInvoked()
        {
            var result = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationContactRequest(_requestNoIssues));
        }
        

        [Test]
        public void GetOrganisationContactDetailsWhenOrganisationContactCreated()
        {
            _returnedUserName = _createEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _returnedUserName.Should().BeEquivalentTo(_expectedOrganisationContactNoIssues.Username);
        }

        [Test]
        public void GeExceptionWheValidationOccurs()
        {
            const string errorMessage = "error happened";
            var requestFailedContactDetails = BuildRequest(_organisationId,_firstName,_lastName, _email, _phoneNumber);
            var errorResponse = BuildErrorResponse(errorMessage, ValidationStatusCode.BadRequest);
            _validator.Setup(v => v.ValidatorCreateEpaOrganisationContactRequest(requestFailedContactDetails)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _createEpaOrganisationContactHandler.Handle(requestFailedContactDetails, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>()), Times.Never);
            _validator.Verify(v => v.ValidatorCreateEpaOrganisationContactRequest(requestFailedContactDetails));
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage, statusCode));
            return validationResponse;
        }

        private CreateEpaOrganisationContactRequest BuildRequest(string organisationId,  string firstName, string lastName,string email, string phoneNumber)
        {
            return new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = organisationId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }

        private EpaContact BuildOrganisationContact(CreateEpaOrganisationContactRequest request, string username)
        {
            return new EpaContact
            {
                Id = Guid.NewGuid(),
                EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId,
                DisplayName = $"{request.FirstName} {request.LastName}",
                Username = username,
                Email = request.Email,
                Status = "Live",
                PhoneNumber = request.PhoneNumber

            };
        }
    }
}
