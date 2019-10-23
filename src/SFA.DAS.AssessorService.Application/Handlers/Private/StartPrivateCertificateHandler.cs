using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Private
{
    public class StartPrivateCertificateHandler : IRequestHandler<StartCertificatePrivateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;             
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;

        public StartPrivateCertificateHandler(ICertificateRepository certificateRepository,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;            
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(StartCertificatePrivateRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            var certificate = await _certificateRepository.GetPrivateCertificate(request.Uln,
                organisation.EndPointAssessorOrganisationId);
            if (certificate != null)
            {
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                if (certificateData.LearnerFamilyName == request.LastName)
                {
                    _logger.LogInformation("Handle Before Update Cert in db");
                    certificate.Status = CertificateStatus.Draft;
                    return await _certificateRepository.Update(certificate, request.Username, null);
                }
            }

            return await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificatePrivateRequest request)
        {
            try
            {
                var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

                var certData = new CertificateData()
                {
                    LearnerFamilyName = request.LastName,
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
                };

                _logger.LogInformation("CreateNewCertificate Before create new Certificate");
                var newCertificate = await _certificateRepository.NewPrivate(
                    new Certificate()
                    {
                        Uln = request.Uln,
                        OrganisationId = organisation.Id,
                        CreatedBy = request.Username,
                        CertificateData = JsonConvert.SerializeObject(certData),
                        Status = CertificateStatus.Draft,
                        CertificateReference = "",
                        CreateDay = DateTime.UtcNow.Date,
                        IsPrivatelyFunded = true
                    }, organisation.EndPointAssessorOrganisationId);

                newCertificate.CertificateReference = newCertificate.CertificateReferenceId.ToString().PadLeft(8, '0');

                // need to update EPA Reference too
                certData.EpaDetails.EpaReference = newCertificate.CertificateReference;
                newCertificate.CertificateData = JsonConvert.SerializeObject(certData);

                _logger.LogInformation("CreateNewCertificate Before Update Cert in db");
                await _certificateRepository.Update(newCertificate, request.Username, null);

                _logger.LogInformation(LoggingConstants.CertificateStarted);
                _logger.LogInformation($"Certificate with ID: {newCertificate.Id} Started with reference of {newCertificate.CertificateReference}");

                return newCertificate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public interface ICommitmentsApi
    {
        CommitmentEmployerDetails GetCommitmentEmployerDetails(long providerId, long commitmentId);
    }

    public class CommitmentEmployerDetails
    {
        public string LegalEntityName { get; set; }
        public string LegalEntityAddress { get; set; }
    }
}
