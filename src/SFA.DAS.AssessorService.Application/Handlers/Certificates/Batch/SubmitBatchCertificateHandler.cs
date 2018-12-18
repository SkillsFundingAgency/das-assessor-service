﻿using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using System.Linq;
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
            var submittedCeriticate = await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Submit);

            return await ApplyStatusInformation(submittedCeriticate);
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

        private async Task<Certificate> ApplyStatusInformation(Certificate certificate)
        {
            // certificate is track-able entity. So we have to this do to stop it from updating in the database
            var json = JsonConvert.SerializeObject(certificate);
            var cert = JsonConvert.DeserializeObject<Certificate>(json);

            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(cert.Id);
            certificateLogs = certificateLogs?.Where(l => l.ReasonForChange is null).ToList(); // this removes any admin changes done within staff app

            var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);
            if (createdLogEntry != null)
            {
                var createdContact = await _contactQueryRepository.GetContact(createdLogEntry.Username);
                cert.CreatedAt = createdLogEntry.EventTime.UtcToTimeZoneTime();
                cert.CreatedBy = createdContact != null ? createdContact.DisplayName : createdLogEntry.Username;
            }

            var submittedLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);

            // NOTE: THIS IS A DATA FRIG FOR EXTERNAL API AS WE NEED SUBMITTED INFORMATION!
            if (submittedLogEntry != null)
            {
                var submittedContact = await _contactQueryRepository.GetContact(submittedLogEntry.Username);
                cert.UpdatedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime();
                cert.UpdatedBy = submittedContact != null ? submittedContact.DisplayName : submittedLogEntry.Username;
            }
            else
            {
                cert.UpdatedAt = null;
                cert.UpdatedBy = null;
            }

            return cert;
        }
    }
}
