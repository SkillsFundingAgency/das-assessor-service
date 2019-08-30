using System;
using NUnit.Framework;
using System.Threading;
using FizzWare.NBuilder;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Notifications.Api.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    public class WhenSendOrganisationDetailsAmendedEmail
    {
        private SendOrganisationDetailsAmendedEmailHandler _sut;
        private SendOrganisationDetailsAmendedEmailRequest _request;
        private Mock<IMediator> _mediator;
        private Mock<IEMailTemplateQueryRepository> _eMailTemplateQueryRepository;
        private Mock<ILogger<SendOrganisationDetailsAmendedEmailHandler>> _logger;

        private EpaOrganisation EpaOrganisation;
        private ContactsPrivilege ChangeOrganisationDetailsContactsPrivilege;
        private ContactsPrivilege ManageUsersContactsPrivilege;
        private EMailTemplate _eMailTemplate;

        private Guid UserId = Guid.NewGuid();
        private Guid ManageUsersPrivilegeId = Guid.NewGuid();
        private Guid ChangeOrganisationPrivilegeId = Guid.NewGuid();

        private ContactsWithPrivilegesResponse _firstContact;
        private ContactsWithPrivilegesResponse _secondContact;
        private ContactsWithPrivilegesResponse _thirdContact;

        private List<ContactResponse> _result;
        private string PropertyChangedName = "PropertyName";
        private string ValueAddedValue = "Value";
        private string EditorCommon;

        private List<string> _emailRequestsSent = null;

        public WhenSendOrganisationDetailsAmendedEmail()
        {
            ChangeOrganisationDetailsContactsPrivilege = new ContactsPrivilege
            {
                Privilege = new Privilege
                {
                    Id = ChangeOrganisationPrivilegeId,
                    Key = Privileges.ChangeOrganisationDetails
                },
                PrivilegeId = ChangeOrganisationPrivilegeId
            };

            ManageUsersContactsPrivilege = new ContactsPrivilege
            {
                Privilege = new Privilege
                {
                    Id = ManageUsersPrivilegeId,
                    Key = Privileges.ManageUsers
                },
                PrivilegeId = ManageUsersPrivilegeId
            };
        }

        [SetUp]
        public void Arrange()
        {
            EpaOrganisation = new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                OrganisationId = "EPA0001"
            };

            _eMailTemplate = new EMailTemplate
            {
                TemplateName = "EmailTemplate"
            };

            _eMailTemplateQueryRepository = new Mock<IEMailTemplateQueryRepository>();
            _eMailTemplateQueryRepository.Setup(c => c.GetEmailTemplate(It.IsAny<string>()))
                .ReturnsAsync(_eMailTemplate);

            _mediator = new Mock<IMediator>();

            _mediator.Setup(c => c.Send(
                It.Is<GetAssessmentOrganisationRequest>(p => p.OrganisationId == EpaOrganisation.OrganisationId),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(EpaOrganisation);

            _firstContact = new ContactsWithPrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "first@organisation.com",
                    DisplayName = "First Person"
                }
            };
            _firstContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ChangeOrganisationDetailsContactsPrivilege.Privilege.Key
            });

            _secondContact = new ContactsWithPrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "second@organisation.com",
                    DisplayName = "Second Person",
                    GivenNames = "Second"
                }
            };
            _secondContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ChangeOrganisationDetailsContactsPrivilege.Privilege.Key
            });
            _secondContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ManageUsersContactsPrivilege.Privilege.Key
            });

            _thirdContact = new ContactsWithPrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "third@organisation.com",
                    DisplayName = "Third Person",
                    GivenNames = "Third"
                }
            };
            _thirdContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ManageUsersContactsPrivilege.Privilege.Key
            });

            _mediator.Setup(c => c.Send(
                    It.IsAny<GetContactsWithPrivilegesRequest>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new List<ContactsWithPrivilegesResponse>
                {
                    _firstContact,
                    _secondContact,
                    _thirdContact
                });

            EditorCommon = _thirdContact.Contact.DisplayName;

            _logger = new Mock<ILogger<SendOrganisationDetailsAmendedEmailHandler>>();

            // this callback is being used to capture the send email requests which are later
            // compared; this is done like this due to the anonymous object for Tokens
            // which cannot be verified in the normal style - NOTE: also the
            // Returns(Task.CompletedTask) which is necessary for a null returning async
            // Mock which is handled by a callback to avoid a NullReferenceException
            // during testing
            _emailRequestsSent = new List<string>();
            _mediator
                .Setup(c => 
                    c.Send(
                        It.IsAny<SendEmailRequest>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<IRequest, CancellationToken>((request, token) =>
                {
                    var sendEmailRequest = request as SendEmailRequest;
                    _emailRequestsSent.Add(JsonConvert.SerializeObject(new
                    {
                        sendEmailRequest.Email,
                        EmailTemplate = sendEmailRequest.EmailTemplate.TemplateName,
                        sendEmailRequest.Tokens
                    }));
                });

            _sut = new SendOrganisationDetailsAmendedEmailHandler(
                _eMailTemplateQueryRepository.Object, _mediator.Object, _logger.Object);
        } 

        private async Task Act()
        {
            _request = new SendOrganisationDetailsAmendedEmailRequest
            {
                OrganisationId = EpaOrganisation.OrganisationId,
                PropertyChanged = PropertyChangedName,
                ValueAdded = ValueAddedValue,
                Editor = EditorCommon
            };

            _result = await _sut.Handle(_request, CancellationToken.None);
        }

        [Test]
        public async Task Then_Contacts_Are_Retrieved_For_Organisation()
        {
            await Act();

            _mediator.Verify(p => p.Send(
                It.Is<GetContactsWithPrivilegesRequest>(c => c.OrganisationId == EpaOrganisation.Id), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Then_Return_Contacts_With_ManageUsers_Permission()
        {
            await Act();

            _result.Count.Should().Be(2);
            _result.Should().OnlyContain(p => 
                p.Email == _secondContact.Contact.Email ||
                p.Email == _thirdContact.Contact.Email);
        }

        [Test]
        public async Task Then_Email_Is_Requested_For_ManageUsers()
        {
            await Act();

            // a callback has captured the email send request; this is being done
            // to check the actual value of the anonymous type which defines the
            // tokens which cannot be done using a normal Verify call due to 
            // expression tree must not contain anonymous types exceptions
            var expectedSecondContactEmail = JsonConvert.SerializeObject(new
            {
                _secondContact.Contact.Email,
                EmailTemplate = _eMailTemplate.TemplateName,
                Tokens = new
                {
                    Contact = _secondContact.Contact.GivenNames,
                    Organisation = EpaOrganisation.Name,
                    PropertyChanged = PropertyChangedName,
                    ValueAdded = ValueAddedValue,
                    ServiceTeam = "Apprenticeship assessment services team",
                    Editor = EditorCommon
                }
            });

            var expectedThirdContactEmail = JsonConvert.SerializeObject(new
            {
                _thirdContact.Contact.Email,
                EmailTemplate = _eMailTemplate.TemplateName,
                Tokens = new
                {
                    Contact = _thirdContact.Contact.GivenNames,
                    Organisation = EpaOrganisation.Name,
                    PropertyChanged = PropertyChangedName,
                    ValueAdded = ValueAddedValue,
                    ServiceTeam = "Apprenticeship assessment services team",
                    Editor = EditorCommon
                }
            });

            // verify that the email send requests contain requests which match those
            // above; the order of the definition of the types above must match the 
            // callback in the Arrange method for the comparision to succeed
            // the serialization comparison is being done to allow comparison of anonymous 
            // types for the Token collection to succeed when those anonymous types are generated
            // from different assemblies
            _emailRequestsSent.Should().Contain(new List<string> { expectedSecondContactEmail, expectedThirdContactEmail });
        }
    }
}
