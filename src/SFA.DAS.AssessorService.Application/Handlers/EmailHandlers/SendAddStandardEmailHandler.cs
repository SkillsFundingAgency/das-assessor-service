using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailHandlers
{
    public class SendAddStandardEmailHandler : IRequestHandler<SendAddStandardEmailRequest>
    {
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStandardRepository _standardRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<SendOrganisationDetailsAmendedEmailHandler> _logger;

        public SendAddStandardEmailHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository,
            IStandardRepository standardRepository, IMediator mediator, ILogger<SendOrganisationDetailsAmendedEmailHandler> logger)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardRepository = standardRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendAddStandardEmailRequest request, CancellationToken cancellationToken)
        {
            var contactToNotify = Guid.TryParse(request.ContactId, out Guid contactId)
                ? await _contactQueryRepository.GetContactById(contactId)
                : null;

            if (contactToNotify == null)
            {
                throw new Exception($"Unable to send email for add standard, cannot find contact {request.ContactId}");
            }

            var standard = (await _standardRepository.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .FirstOrDefault();
            
            if (standard == null)
            {
                throw new Exception($"Unable to send email for add standard, cannot find standard reference {request.StandardReference}");
            }

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOStandardAdd);
            if (emailTemplate == null)
            {
                throw new Exception($"Unable to send email for add standard, cannot find email template {EmailTemplateNames.EPAOStandardAdd}");
            }

            var standardversioninfo = $"version{(request.StandardVersions.Count > 1 ? "s" : string.Empty)} {string.Join(", ", request.StandardVersions)}";

            await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                new { standardreference = request.StandardReference, standard = standard.Title, standardversioninfo, contactname = contactToNotify.DisplayName }), cancellationToken);

            return Unit.Value;
        }
    }
}
