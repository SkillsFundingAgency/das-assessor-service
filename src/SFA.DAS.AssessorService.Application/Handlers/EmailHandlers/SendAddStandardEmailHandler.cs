using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Exceptions;
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
        private readonly IStandardService _standardService;
        private readonly IMediator _mediator;
        private readonly ILogger<SendAddStandardEmailHandler> _logger;

        public SendAddStandardEmailHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository,
            IStandardService standardService, IMediator mediator, ILogger<SendAddStandardEmailHandler> logger)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardService = standardService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendAddStandardEmailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Sending email to notify that standard {request.StandardReference} has been added by contact {request.ContactId}");

            var contactToNotify = Guid.TryParse(request.ContactId, out Guid contactId)
                ? await _contactQueryRepository.GetContactById(contactId)
                : null;

            if (contactToNotify == null)
            {
                throw new NotFoundException($"Unable to send email for add standard, cannot find ContactId {request.ContactId}");
            }

            var standard = (await _standardService.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .FirstOrDefault();
            
            if (standard == null)
            {
                throw new NotFoundException($"Unable to send email for add standard, cannot find StandardReference {request.StandardReference}");
            }

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOStandardAdd);
            if (emailTemplate == null)
            {
                throw new NotFoundException($"Unable to send email for add standard, cannot find email template {EmailTemplateNames.EPAOStandardAdd}");
            }

            var standardversioninfo = $"version{(request.StandardVersions.Count > 1 ? "s" : string.Empty)} {string.Join(", ", request.StandardVersions)}";

            await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                new { standardreference = request.StandardReference, standard = standard.Title, standardversioninfo, contactname = contactToNotify.DisplayName }), cancellationToken);

            return Unit.Value;
        }
    }
}
