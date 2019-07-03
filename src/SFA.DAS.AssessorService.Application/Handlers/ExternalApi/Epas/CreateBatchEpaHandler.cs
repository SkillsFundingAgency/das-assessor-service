using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas
{
    public class CreateBatchEpaHandler : IRequestHandler<CreateBatchEpaRequest, EpaDetails>
    {
        private readonly IMediator _mediator;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<CreateBatchEpaHandler> _logger;

        public CreateBatchEpaHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, ILogger<CreateBatchEpaHandler> logger, IMediator mediator)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<EpaDetails> Handle(CreateBatchEpaRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewEpa(request);
        }

        private async Task<EpaDetails> CreateNewEpa(CreateBatchEpaRequest request)
        {
            _logger.LogInformation("CreateNewEpa Before Get Contact from API");
            var contact = await GetContactFromEmailAddress(request.Email);

            _logger.LogInformation("CreateNewEpa Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if(certificate is null)
            {
                _logger.LogInformation("CreateNewEpa Before StartCertificateRequest");
                var startCertificateRequest = new StartCertificateRequest { StandardCode = request.StandardCode, UkPrn = request.UkPrn, Uln = request.Uln, Username = contact.Username };
                certificate = await _mediator.Send(startCertificateRequest);
            }

            if (certificate is null) throw new NotFound();

            _logger.LogInformation("CreateNewEpa Before Combining EpaDetails");
            var latestRecord = request.EpaDetails.Epas.OrderByDescending(epa => epa.EpaDate).First();
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certData.EpaDetails = request.EpaDetails ?? new EpaDetails();
            certData.EpaDetails.EpaReference = certificate.CertificateReference;
            certData.EpaDetails.LatestEpaDate = latestRecord.EpaDate;
            certData.EpaDetails.LatestEpaOutcome = latestRecord.EpaOutcome;

            _logger.LogInformation("CreateNewEpa Before Update CertificateData");
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            _logger.LogInformation("CreateNewEpa Before Update Cert in db");
            await _certificateRepository.Update(certificate, contact.Username, CertificateActions.Epa);

            return certData.EpaDetails;
        }

        private async Task<Contact> GetContactFromEmailAddress(string email)
        {
            Contact contact = await _contactQueryRepository.GetContactFromEmailAddress(email);

            if( contact == null)
            {
                contact = new Contact { Username = email, Email = email };
            }

            return contact;
        }
    }
}
