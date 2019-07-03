using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class DeleteBatchEpaHandler : IRequestHandler<DeleteBatchEpaRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<DeleteBatchEpaHandler> _logger;

        public DeleteBatchEpaHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<DeleteBatchEpaHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task Handle(DeleteBatchEpaRequest request, CancellationToken cancellationToken)
        {
            await DeleteEpaDetails(request);
        }

        private async Task DeleteEpaDetails(DeleteBatchEpaRequest request)
        {
            _logger.LogInformation("DeleteEpaDetails Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("DeleteEpaDetails Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null) throw new NotFound();

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            _logger.LogInformation("DeleteEpaDetails Before Update CertificateData");
            certificate.Status = CertificateStatus.Deleted;
            certData.EpaDetails = null;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            _logger.LogInformation("DeleteEpaDetails Before Update Cert in db");
            await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Delete);
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
