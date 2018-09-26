using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Private
{
    public class StartPrivateCertificateHandler : IRequestHandler<StartCertificatePrivateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;

        public StartPrivateCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(StartCertificatePrivateRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            var certificate = await _certificateRepository.GetPrivateCertificate(request.Uln,
                organisation.EndPointAssessorOrganisationId, request.LastName);
            if (certificate != null)
            {
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                if (certificateData.LearnerFamilyName != request.LastName)
                {                    
                    certificateData.LearnerFamilyName = request.LastName;
                    if (!string.IsNullOrEmpty(certificateData.LearnerGivenNames))
                    {
                        certificateData.FullName = certificateData.LearnerGivenNames + ' ' + request.LastName;
                    }
                    certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
                    certificate = await _certificateRepository.Update(certificate,
                        request.Username,
                        CertificateActions.Name);
                }
                return certificate;
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
                    LearnerFamilyName = request.LastName
                };

                _logger.LogInformation("CreateNewCertificate Before create new Certificate");
                var newCertificate = await _certificateRepository.NewPrivate(
                    new Certificate()
                    {
                        Uln = request.Uln,
                        OrganisationId = organisation.Id,
                        CreatedBy = request.Username,
                        CertificateData = JsonConvert.SerializeObject(certData),
                        Status = Domain.Consts.CertificateStatus.Draft,
                        CertificateReference = "",
                        CreateDay = DateTime.UtcNow.Date,
                        IsPrivatelyFunded = true
                    }, organisation.EndPointAssessorOrganisationId);

                newCertificate.CertificateReference = newCertificate.CertificateReferenceId.ToString().PadLeft(8, '0');

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
            return null;
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
