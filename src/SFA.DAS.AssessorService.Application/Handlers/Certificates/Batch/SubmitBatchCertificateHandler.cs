using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class SubmitBatchCertificateHandler : IRequestHandler<SubmitBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<SubmitBatchCertificateHandler> _logger;

        public SubmitBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<SubmitBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(SubmitBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await SubmitCertificate(request);
        }

        private async Task<Certificate> SubmitCertificate(SubmitBatchCertificateRequest request)
        {
            _logger.LogInformation("SubmitCertificate Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("SubmitCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            _logger.LogInformation("SubmitCertificate Before Update Certificate Status");
            certificate.Status = CertificateStatus.Submitted;

            _logger.LogInformation("SubmitCertificate Before Update Cert in db");
            return await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Submit);
        }

        private async Task<Contact> GetContactFromEmailAddress(string email)
        {
            Contact contact = await _contactQueryRepository.GetContactFromEmailAddress(email);

            if (contact == null)
            {
                contact = new Contact { Username = email, Email = email };
            }

            return contact;
        }
    }
}
