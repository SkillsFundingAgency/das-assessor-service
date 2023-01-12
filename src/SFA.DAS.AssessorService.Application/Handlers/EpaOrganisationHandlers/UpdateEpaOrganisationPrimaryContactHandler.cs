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
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<UpdateEpaOrganisationPrimaryContactHandler> _logger;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationPrimaryContactHandler(IContactQueryRepository contactQueryRepository, ILogger<UpdateEpaOrganisationPrimaryContactHandler> logger,
            IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationPrimaryContactRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var success = await _mediator.Send(new AssociateEpaOrganisationWithEpaContactRequest
            {
                ContactId = request.PrimaryContactId,
                OrganisationId = organisation.OrganisationId,
                ContactStatus = ContactStatus.Live,
                MakePrimaryContact = true,
                AddDefaultPrivileges = false
            });

            if (success)
            {
                var primaryContact = await _contactQueryRepository.GetContactById(request.PrimaryContactId);
                var updatedBy = request.UpdatedBy.HasValue
                    ? await _contactQueryRepository.GetContactById(request.UpdatedBy.Value)
                    : null;

                try
                {
                    var primaryContactaAmendedEmailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOPrimaryContactAmended);
                    if (primaryContactaAmendedEmailTemplate != null)
                    {
                        _logger.LogInformation($"Sending email to notify updated primary contact {primaryContact.Username} for organisation {organisation.Name}");

                        await _mediator.Send(new SendEmailRequest(primaryContact.Email,
                            primaryContactaAmendedEmailTemplate, new
                            {
                                Contact = primaryContact.GivenNames,
                                ServiceName = "Apprenticeship assessment service",
                                Organisation = organisation.Name,
                                ServiceTeam = "Apprenticeship assessment services team",
                                Editor = updatedBy?.DisplayName ?? "EFSA Staff"
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
                    Editor = updatedBy?.DisplayName ?? "ESFA Staff"
                });
            }

            return null;
        }
    }
}
