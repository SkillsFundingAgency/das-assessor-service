using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class GetBatchCertificateHandler : IRequestHandler<GetBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<GetBatchCertificateHandler> _logger;
        private readonly IStandardRepository _standardRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetBatchCertificateHandler(ICertificateRepository certificateRepository, IContactQueryRepository contactQueryRepository, IStandardRepository standardRepository, IOrganisationQueryRepository organisationQueryRepository, ILogger<GetBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardRepository = standardRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(GetBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await GetCertificate(request);
        }

        private async Task<Certificate> GetCertificate(GetBatchCertificateRequest request)
        {
            _logger.LogInformation("GetCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode, request.FamilyName, request.IncludeLogs);

            _logger.LogInformation("GetCertificate Before Get Searching Organisation from db");
            var searchingOrganisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            var allowedCertificateStatus = new[] { CertificateStatus.Draft, CertificateStatus.Submitted }.Concat(CertificateStatus.PrintProcessStatus);

            if (certificate == null || searchingOrganisation == null || !allowedCertificateStatus.Contains(certificate.Status))
            {
                return null;
            }

            certificate = await CertificateHelpers.ApplyStatusInformation(_certificateRepository, _contactQueryRepository, certificate);

            if ((certificate.Status == CertificateStatus.Submitted || CertificateStatus.HasPrintProcessStatus(certificate.Status)) && 
                certificate.CertificateData.OverallGrade == CertificateGrade.Fail)
            {
                return null;
            }
            else if (certificate.Status == CertificateStatus.Draft && 
                EpaOutcome.Pass.Equals(certificate.CertificateData.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase) &&
                certificate.CertificateData.OverallGrade == null)
            {
                return null;
            }

            if (certificate.OrganisationId != searchingOrganisation.Id)
            {
                var providedStandards = await _standardRepository.GetEpaoRegisteredStandards(searchingOrganisation.EndPointAssessorOrganisationId, int.MaxValue, 1);

                if (providedStandards.PageOfResults.Any(s => s.StandardCode == certificate.StandardCode))
                {
                    // Shared standard but not the EPAO who created the certificate
                    certificate = RedactCertificateInformation(certificate, false);
                }
                else
                {
                    certificate = null;
                }
            }
            
            return certificate;
        }

        private Certificate RedactCertificateInformation(Certificate certificate, bool showEpaDetails)
        {
            // certificate is track-able entity. So we have to do this in order to stop it from updating in the database
            var json = JsonConvert.SerializeObject(certificate);
            var cert = JsonConvert.DeserializeObject<Certificate>(json);

            CertificateData redactedData = new CertificateData
            {
                LearnerGivenNames = certificate.CertificateData.LearnerGivenNames,
                LearnerFamilyName = certificate.CertificateData.LearnerFamilyName,
                StandardReference = certificate.CertificateData.StandardReference,
                StandardName = certificate.CertificateData.StandardName,
                StandardLevel = certificate.CertificateData.StandardLevel,
                StandardPublicationDate = certificate.CertificateData.StandardPublicationDate,
                EpaDetails = showEpaDetails ? certificate.CertificateData.EpaDetails : null
            };

            cert.CertificateData = redactedData;
            cert.CertificateReference = null;
            cert.CertificateReferenceId = null;
            cert.CreateDay = DateTime.MinValue;
            cert.CreatedAt = DateTime.MinValue;
            cert.CreatedBy = null;
            cert.UpdatedAt = null;
            cert.UpdatedBy = null;
            cert.DeletedBy = null;
            cert.DeletedAt = null;
            cert.BatchNumber = null;
            cert.ToBePrinted = null;

            return cert;
        }
    }
}
