using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Auditing;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Command
{
    [TestFixture]
    public class RegisterUpdateOrganisationContactHandlerTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private UpdateEpaOrganisationContactHandler _updateEpaOrganisationContactHandler;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IEpaOrganisationValidator> _validator;
        private Mock<ILogger<UpdateEpaOrganisationContactHandler>> _logger;
        private Mock<IMediator> _mediator;
        private Mock<IAuditLogService> _auditLogService;
        private UpdateEpaOrganisationContactRequest _requestNoIssues;
        private string _organisationId;
        private Guid _contactId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private EpaContact _expectedOrganisationContactNoIssues;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _logger = new Mock<ILogger<UpdateEpaOrganisationContactHandler>>();
            _mediator = new Mock<IMediator>();
            _auditLogService = new Mock<IAuditLogService>();

            _organisationId = "EPA0001";
            _contactId = Guid.NewGuid();
            _email = "test@testy.com";
            _firstName = "Joe";
            _lastName = "Cool";
            _phoneNumber = "555 4444";

            _requestNoIssues = BuildRequest(_contactId, _firstName,_lastName, _email,_phoneNumber);
            _expectedOrganisationContactNoIssues = BuildOrganisationStandard(_requestNoIssues);

            _registerRepository.Setup(r => r.UpdateEpaOrganisationContact(It.IsAny<EpaContact>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(_expectedOrganisationContactNoIssues.Id.ToString()));
            
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _validator.Setup(v => v.ValidatorUpdateEpaOrganisationContactRequest(_requestNoIssues))
                .Returns(new ValidationResponse());

            _mediator.Setup(v => v.Send(It.IsAny<GetEpaContactRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EpaContact
                {
                    Id = _contactId,
                    EndPointAssessorOrganisationId = _organisationId
                });

            _auditLogService.Setup(v => v.GetEpaOrganisationPrimaryContactChanges(It.IsAny<string>(), It.IsAny<EpaContact>()))
                .ReturnsAsync(new List<AuditChange>());
                
            _updateEpaOrganisationContactHandler = new UpdateEpaOrganisationContactHandler(_registerRepository.Object, _validator.Object, _mediator.Object,
                _auditLogService.Object, _cleanserService.Object, _logger.Object);
        }
        
        [Test]
        public void UpdateOrganisationContactDetailsRepoIsCalledWhenHandlerInvoked()
        {
            var res = _updateEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _registerRepository.Verify(r => r.UpdateEpaOrganisationContact(It.IsAny<EpaContact>(), It.IsAny<bool>()));
        }

        [Test]
        public void CheckValidatorIsCalledWhenHandlerInvoked()
        {
            var res = _updateEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            _validator.Verify(v => v.ValidatorUpdateEpaOrganisationContactRequest(_requestNoIssues));
        }

        [Test]
        public void GetOrganisationStandardWhenOrganisationStandardUpdated()
        {
            var returnedId = _updateEpaOrganisationContactHandler.Handle(_requestNoIssues, new CancellationToken()).Result;
            returnedId.Should().Be(_expectedOrganisationContactNoIssues.Id.ToString());
        }
        
        [Test]
        public void GeExceptionWheValidationOccurs()
        {
            const string errorMessage = "error happened";
            var requestFailedContactDetails = BuildRequest(_contactId, _firstName, _lastName, _email, _phoneNumber);
            var errorResponse = BuildErrorResponse(errorMessage, ValidationStatusCode.BadRequest);
            _validator.Setup(v => v.ValidatorUpdateEpaOrganisationContactRequest(requestFailedContactDetails)).Returns(errorResponse);
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _updateEpaOrganisationContactHandler.Handle(requestFailedContactDetails, new CancellationToken()));
            Assert.AreEqual(errorMessage + "; ", ex.Message);
            _registerRepository.Verify(r => r.UpdateEpaOrganisationContact(It.IsAny<EpaContact>(),It.IsAny<bool>()), Times.Never);
            _validator.Verify(v => v.ValidatorUpdateEpaOrganisationContactRequest(requestFailedContactDetails));
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage, statusCode));
            return validationResponse;
        }

        private EpaContact BuildOrganisationStandard(UpdateEpaOrganisationContactRequest requestNoIssues)
        {
            return new EpaContact
            {
                Id = Guid.Parse(requestNoIssues.ContactId),
                DisplayName = $"{requestNoIssues.FirstName} {requestNoIssues.LastName}",
                Email = requestNoIssues.Email,
                PhoneNumber = requestNoIssues.PhoneNumber
            };
        }

        private UpdateEpaOrganisationContactRequest BuildRequest(Guid contactId, string firstName, string lastName, string email, string phoneNumber)
        {
            return new UpdateEpaOrganisationContactRequest
            {
                ContactId = contactId.ToString(),
                FirstName = firstName,
                LastName =  lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }
    }
}