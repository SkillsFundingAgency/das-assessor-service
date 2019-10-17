using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailHandlers
{
    public class SendOrganisationDetailsAmendedEmailHandler : IRequestHandler<SendOrganisationDetailsAmendedEmailRequest, List<ContactResponse>>
    {
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<SendOrganisationDetailsAmendedEmailHandler> _logger;

        public SendOrganisationDetailsAmendedEmailHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository,
            IMediator mediator, ILogger<SendOrganisationDetailsAmendedEmailHandler> logger)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<List<ContactResponse>> Handle(SendOrganisationDetailsAmendedEmailRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            try
            {
                var contactsWithPrivileges = await _mediator.Send(new GetContactsWithPrivilegesRequest(organisation.Id));
                var contactsWithManageUserPrivilege = contactsWithPrivileges?
                    .Where(c => 
                        c.Privileges.Any(p => p.Key == Privileges.ManageUsers) && 
                        (c.Contact.Status == ContactStatus.Live || c.Contact.Status == ContactStatus.Active))
                    .ToList();

                var organisationDetailsAmendedEmailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOOrganisationDetailsAmended);
                if (organisationDetailsAmendedEmailTemplate != null)
                {
                    foreach (var contactWithManageUserPrivilege in contactsWithManageUserPrivilege)
                    {
                        _logger.LogInformation($"Sending email to notify amended {request.PropertyChanged} {request.ValueAdded} for organisation {organisation.Name} to {contactWithManageUserPrivilege.Contact.Email}");

                        var sendEmailRequest = new SendEmailRequest(contactWithManageUserPrivilege.Contact.Email,
                            organisationDetailsAmendedEmailTemplate, new
                            {
                                Contact = $"{contactWithManageUserPrivilege.Contact.GivenNames}",
                                Organisation = organisation.Name,
                                request.PropertyChanged,
                                request.ValueAdded,
                                ServiceTeam = "Apprenticeship assessment services team",
                                request.Editor
                            });

                        await _mediator.Send(sendEmailRequest, cancellationToken);
                    }
                }

                return contactsWithManageUserPrivilege
                    .Select(x => x.Contact)
                    .ToList();
            }
            catch (Exception)
            {
                _logger.LogInformation($"Unable to send email to notify amended {request.PropertyChanged} {request.ValueAdded} for organisation {organisation.Name} to contacts with mangage user privileges");
            }

            return null;
        }
    }
}
