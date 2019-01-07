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
    public class DeleteBatchCertificateHandler : IRequestHandler<DeleteBatchCertificateRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<DeleteBatchCertificateHandler> _logger;

        public DeleteBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<DeleteBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task Handle(DeleteBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            await DeleteCertificate(request);
        }

        private async Task DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            _logger.LogInformation("SubmitCertificate Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("DeleteCertificate Before set Certificate to Deleted in db");
            await _certificateRepository.Delete(request.Uln, request.StandardCode, contact.Username, CertificateActions.Delete);
            _logger.LogInformation("DeleteCertificate Certificate set to Deleted in db");
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
