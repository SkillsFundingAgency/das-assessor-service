using System;
using NUnit.Framework;
using System.Threading;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    public class SendOrganisationDetailsAmendedEmailHandlerTests
    {
        private SendOrganisationDetailsAmendedEmailHandler _sut;
        private SendOrganisationDetailsAmendedEmailRequest _request;
        private Mock<IMediator> _mediator;
        private Mock<IEMailTemplateQueryRepository> _eMailTemplateQueryRepository;
        private Mock<ILogger<SendOrganisationDetailsAmendedEmailHandler>> _logger;

        private EpaOrganisation EpaOrganisation;
        private ContactsPrivilege ChangeOrganisationDetailsContactsPrivilege;
        private ContactsPrivilege ManageUsersContactsPrivilege;
        private EmailTemplateSummary _eMailTemplateSummary;

        private Guid UserId = Guid.NewGuid();
        private Guid ManageUsersPrivilegeId = Guid.NewGuid();
        private Guid ChangeOrganisationPrivilegeId = Guid.NewGuid();

        private ContactIncludePrivilegesResponse _firstLiveContact;
        private ContactIncludePrivilegesResponse _secondLiveContact;
        private ContactIncludePrivilegesResponse _thirdLiveContact;
        private ContactIncludePrivilegesResponse _firstPendingContact;

        private List<ContactResponse> _result;
        private string PropertyChangedName = "PropertyName";
        private string ValueAddedValue = "Value";
        private string EditorCommon;

        private List<string> _emailRequestsSent = null;

        public SendOrganisationDetailsAmendedEmailHandlerTests()
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

            _eMailTemplateSummary = new EmailTemplateSummary
            {
                TemplateName = "EmailTemplate"
            };

            _eMailTemplateQueryRepository = new Mock<IEMailTemplateQueryRepository>();
            _eMailTemplateQueryRepository.Setup(c => c.GetEmailTemplate(It.IsAny<string>()))
                .ReturnsAsync(_eMailTemplateSummary);

            _mediator = new Mock<IMediator>();

            _mediator.Setup(c => c.Send(
                It.Is<GetAssessmentOrganisationRequest>(p => p.OrganisationId == EpaOrganisation.OrganisationId),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(EpaOrganisation);

            _firstLiveContact = new ContactIncludePrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "firstlive@organisation.com",
                    DisplayName = "First Live Person",
                    Status = ContactStatus.Live
                }
            };
            _firstLiveContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ChangeOrganisationDetailsContactsPrivilege.Privilege.Key
            });

            _secondLiveContact = new ContactIncludePrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "secondlive@organisation.com",
                    DisplayName = "Second Live Person",
                    GivenNames = "Second Live",
                    Status = ContactStatus.Live
                }
            };
            _secondLiveContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ChangeOrganisationDetailsContactsPrivilege.Privilege.Key
            });
            _secondLiveContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ManageUsersContactsPrivilege.Privilege.Key
            });

            _thirdLiveContact = new ContactIncludePrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "thirdliveorganisation.com",
                    DisplayName = "Third Live Person",
                    GivenNames = "Third Live",
                    Status = ContactStatus.Live
                }
            };
            _thirdLiveContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ManageUsersContactsPrivilege.Privilege.Key
            });

            _firstPendingContact = new ContactIncludePrivilegesResponse()
            {
                Contact = new ContactResponse
                {
                    Email = "firstpending@organisation.com",
                    DisplayName = "First Pending Person",
                    GivenNames = "First Pending",
                    Status = ContactStatus.InvitePending
                }
            };
            _firstPendingContact.Privileges.Add(new PrivilegeResponse
            {
                Key = ManageUsersContactsPrivilege.Privilege.Key
            });

            _mediator.Setup(c => c.Send(
                    It.IsAny<GetAllContactsIncludePrivilegesRequest>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new List<ContactIncludePrivilegesResponse>
                {
                    _firstLiveContact,
                    _secondLiveContact,
                    _thirdLiveContact,
                    _firstPendingContact
                });

            EditorCommon = _thirdLiveContact.Contact.DisplayName;

            _logger = new Mock<ILogger<SendOrganisationDetailsAmendedEmailHandler>>();

            // this callback is being used to capture the send email requests which are later
            // compared; this is done like this due to the anonymous object for Tokens
            // which cannot be verified in the normal style
            _emailRequestsSent = new List<string>();
            _mediator
                .Setup(c => 
                    c.Send(
                        It.IsAny<SendEmailRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<IRequest<Unit>, CancellationToken>((request, token) =>
                {
                    var sendEmailRequest = request as SendEmailRequest;
                    _emailRequestsSent.Add(JsonConvert.SerializeObject(new
                    {
                        sendEmailRequest.Email,
                        EmailTemplate = sendEmailRequest.EmailTemplateSummary.TemplateName,
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
                It.Is<GetAllContactsIncludePrivilegesRequest>(c => c.EndPointAssessorOrganisationId == EpaOrganisation.OrganisationId), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Then_Return_Contacts_With_ManageUsers_Permission()
        {
            await Act();

            _result.Count.Should().Be(2);
            _result.Should().OnlyContain(p => 
                p.Email == _secondLiveContact.Contact.Email ||
                p.Email == _thirdLiveContact.Contact.Email);
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
                _secondLiveContact.Contact.Email,
                EmailTemplate = _eMailTemplateSummary.TemplateName,
                Tokens = new
                {
                    Contact = _secondLiveContact.Contact.GivenNames,
                    Organisation = EpaOrganisation.Name,
                    PropertyChanged = PropertyChangedName,
                    ValueAdded = ValueAddedValue,
                    ServiceTeam = "Apprenticeship assessment services team",
                    Editor = EditorCommon
                }
            });

            var expectedThirdContactEmail = JsonConvert.SerializeObject(new
            {
                _thirdLiveContact.Contact.Email,
                EmailTemplate = _eMailTemplateSummary.TemplateName,
                Tokens = new
                {
                    Contact = _thirdLiveContact.Contact.GivenNames,
                    Organisation = EpaOrganisation.Name,
                    PropertyChanged = PropertyChangedName,
                    ValueAdded = ValueAddedValue,
                    ServiceTeam = "Apprenticeship assessment services team",
                    Editor = EditorCommon
                }
            });

            var notExpectedFirstContactEmail = JsonConvert.SerializeObject(new
            {
                _firstPendingContact.Contact.Email,
                EmailTemplate = _eMailTemplateSummary.TemplateName,
                Tokens = new
                {
                    Contact = _firstPendingContact.Contact.GivenNames,
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
            _emailRequestsSent.Should().NotContain(new List<string> { notExpectedFirstContactEmail });
        }
    }
}
