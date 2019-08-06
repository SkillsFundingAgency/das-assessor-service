using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class UpdateBatchEpaHandler : IRequestHandler<UpdateBatchEpaRequest, EpaDetails>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<UpdateBatchEpaHandler> _logger;

        public UpdateBatchEpaHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<UpdateBatchEpaHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<EpaDetails> Handle(UpdateBatchEpaRequest request, CancellationToken cancellationToken)
        {
            return await UpdateEpaDetails(request);
        }

        private async Task<EpaDetails> UpdateEpaDetails(UpdateBatchEpaRequest request)
        {
            _logger.LogInformation("UpdateEpaDetails Before Get Contact from db");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("UpdateEpaDetails Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate is null) throw new NotFound();

            _logger.LogInformation("UpdateEpaDetails Before Combining EpaDetails");
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certData.EpaDetails = new EpaDetails { EpaReference = certificate.CertificateReference, Epas = new List<EpaRecord>() };

            if (request.EpaDetails?.Epas != null)
            {
                foreach (var epa in request.EpaDetails.Epas)
                {
                    epa.EpaOutcome = NormalizeEpaOutcome(epa.EpaOutcome);
                    certData.EpaDetails.Epas.Add(epa);
                }
            }

            var latestEpaRecord = certData.EpaDetails.Epas.OrderByDescending(epa => epa.EpaDate).FirstOrDefault();
            certData.EpaDetails.LatestEpaDate = latestEpaRecord?.EpaDate;
            certData.EpaDetails.LatestEpaOutcome = latestEpaRecord?.EpaOutcome;

            _logger.LogInformation("UpdateEpaDetails Before Update CertificateData");
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            _logger.LogInformation("UpdateEpaDetails Before Update Cert in db");
            await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Amend);

            return certData.EpaDetails;
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

        private static string NormalizeEpaOutcome(string epaOutcome)
        {
            var outcomes = new string[] { EpaOutcome.Pass, EpaOutcome.Fail, EpaOutcome.Withdrawn };
            return outcomes.FirstOrDefault(g => g.Equals(epaOutcome, StringComparison.InvariantCultureIgnoreCase)) ?? epaOutcome;
        }
    }
}
