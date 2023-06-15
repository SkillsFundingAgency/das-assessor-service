using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EmailHandlers
{
    public class SendOptInStandardVersionEmailHandler : IRequestHandler<SendOptInStandardVersionEmailRequest>
    {
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStandardService _standardService;
        private readonly IMediator _mediator;
        private readonly ILogger<SendOptInStandardVersionEmailHandler> _logger;

        public SendOptInStandardVersionEmailHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository,
            IStandardService standardService, IMediator mediator, ILogger<SendOptInStandardVersionEmailHandler> logger)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardService = standardService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendOptInStandardVersionEmailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Sending email to notify that standard {request.StandardReference} version {request.Version} has been opted in by contact {request.ContactId}");

            var contactToNotify = await _contactQueryRepository.GetContactById(request.ContactId);
            if (contactToNotify == null)
            {
                throw new NotFoundException($"Unable to send email for opt in standard version, cannot find contact {request.ContactId}");
            }

            var standardVersions = await _standardService.GetStandardVersionsByIFateReferenceNumber(request.StandardReference);
            if (!standardVersions?.Any() ?? false)
            {
                throw new NotFoundException($"Unable to send email for opt in standard version, cannot find version {request.Version} for standard reference {request.StandardReference}");
            }

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.EPAOStandardConfimOptIn);
            if (emailTemplate == null)
            {
                throw new NotFoundException($"Unable to send email for opt in standard version, cannot find email template {EmailTemplateNames.EPAOStandardConfimOptIn}");
            }

            await _mediator.Send(new SendEmailRequest(contactToNotify.Email, emailTemplate,
                new 
                {
                    request.StandardReference, 
                    Standard = standardVersions.FirstOrDefault().Title, 
                    request.Version, 
                    ContactName = contactToNotify.DisplayName,
                    EmailTemplateTokens.ServiceName,
                    EmailTemplateTokens.ServiceTeam
                }), cancellationToken);

            return Unit.Value;
        }
    }
}