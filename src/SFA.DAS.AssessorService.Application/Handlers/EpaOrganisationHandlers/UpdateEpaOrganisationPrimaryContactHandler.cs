using MediatR;
using Microsoft.Extensions.Logging;

using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationPrimaryContactHandler : IRequestHandler<UpdateEpaOrganisationPrimaryContactRequest, List<ContactResponse>>
    { 
        private readonly ILogger<UpdateEpaOrganisationPrimaryContactHandler> _logger;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;
        private readonly IAuditLogService _auditLogService;

        public UpdateEpaOrganisationPrimaryContactHandler(ILogger<UpdateEpaOrganisationPrimaryContactHandler> logger,
            IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator, IAuditLogService auditLogService)
        {
            _logger = logger;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
            _auditLogService = auditLogService;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationPrimaryContactRequest request, CancellationToken cancellationToken)
        {
            var primaryContact = await _mediator.Send(new GetEpaContactRequest { ContactId = request.PrimaryContactId });

            var changes = await _auditLogService.GetEpaOrganisationPrimaryContactChanges(request.OrganisationId, primaryContact);

            var success = await _mediator.Send(new AssociateEpaOrganisationWithEpaContactRequest
            {
                ContactId = request.PrimaryContactId,
                OrganisationId = request.OrganisationId,
                ContactStatus = ContactStatus.Live,
                MakePrimaryContact = true,
                AddDefaultRoles = false,
                AddDefaultPrivileges = false
            });

            if (success)
            {
                var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });
                var updatedByContact = request.UpdatedBy.HasValue
                    ? await _mediator.Send(new GetEpaContactRequest { ContactId = request.UpdatedBy.Value })
                    : null;

                await _auditLogService.WriteChangesToAuditLog(request.OrganisationId, updatedByContact?.DisplayName ?? "Unknown", changes);

                try
                {
                    var primaryContactaAmendedEmailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOPrimaryContactAmended);
                    if (primaryContactaAmendedEmailTemplate != null)
                    {
                        _logger.LogInformation($"Sending email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name}");

                        await _mediator.Send(new SendEmailRequest(primaryContact.Email,
                            primaryContactaAmendedEmailTemplate, new
                            {
                                Contact = primaryContact.FirstName,
                                ServiceName = "Apprenticeship assessment service",
                                Organisation = organisation.Name,
                                ServiceTeam = "Apprenticeship assessment services team",
                                Editor = updatedByContact?.DisplayName ?? "EFSA Staff"
                            }), cancellationToken);
                    }
                }
                catch (Exception)
                {
                    _logger.LogInformation($"Unable to send email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name}");
                }

                return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Contact name",
                    ValueAdded = primaryContact.DisplayName,
                    Editor = updatedByContact?.DisplayName ?? "ESFA Staff"
                });
            }

            return null;
        }
    }
}
