using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class CreateBatchCertificateHandler : IRequestHandler<BatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<CreateBatchCertificateHandler> _logger;

        public CreateBatchCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<CreateBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(BatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.GetCertificate(request.Uln, request.StandardCode) ??
                   await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(BatchCertificateRequest request)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
            var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
            _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);
            _logger.LogInformation("CreateNewCertificate Before Get Standard from API");
            var standard = await _assessmentOrgsApiClient.GetStandard(ilr.StdCode);
            _logger.LogInformation("CreateNewCertificate Before Get Provider from API");
            Provider provider;
            try
            {
                provider = await _assessmentOrgsApiClient.GetProvider(ilr.UkPrn);
            }
            catch (Exception)
            {
                // see whether there are any previous certificates with this ukrpn and a ProviderName....
                var previousProviderName = await _certificateRepository.GetPreviousProviderName(ilr.UkPrn);
                provider = previousProviderName != null
                    ? new Provider { ProviderName = previousProviderName }
                    : new Provider { ProviderName = "Unknown" };
            }

            var certData = new CertificateData()
            {
                LearnerGivenNames = ilr.GivenNames,
                LearnerFamilyName = ilr.FamilyName,
                StandardName = standard.Title,
                LearningStartDate = ilr.LearnStartDate,
                StandardLevel = standard.Level,
                StandardPublicationDate = standard.EffectiveFrom,
                FullName = $"{ilr.GivenNames} {ilr.FamilyName}",
                ProviderName = provider.ProviderName,

                ContactName = request.CertificateData.ContactName,
                ContactOrganisation = request.CertificateData.ContactOrganisation,
                ContactAddLine1 = request.CertificateData.ContactAddLine1,
                ContactAddLine2 = request.CertificateData.ContactAddLine2,
                ContactAddLine3 = request.CertificateData.ContactAddLine3,
                ContactAddLine4 = request.CertificateData.ContactAddLine4,
                ContactPostCode = request.CertificateData.ContactPostCode,
                Registration = request.CertificateData.Registration,
                AchievementDate = request.CertificateData.AchievementDate,
                CourseOption = request.CertificateData.CourseOption,
                OverallGrade = request.CertificateData.OverallGrade,
                Department = request.CertificateData.Department
            };

            _logger.LogInformation("CreateNewCertificate Before create new Certificate");
            var newCertificate = await _certificateRepository.New(
                new Certificate()
                {
                    Uln = request.Uln,
                    StandardCode = request.StandardCode,
                    ProviderUkPrn = ilr.UkPrn,
                    OrganisationId = organisation.Id,
                    CreatedBy = request.Username,
                    CertificateData = JsonConvert.SerializeObject(certData),
                    Status = Domain.Consts.CertificateStatus.Draft,
                    CertificateReference = "",
                    LearnRefNumber = ilr.LearnRefNumber
                });

            newCertificate.CertificateReference = newCertificate.CertificateReferenceId.ToString().PadLeft(8, '0');

            _logger.LogInformation("CreateNewCertificate Before Update Cert in db");
            await _certificateRepository.Update(newCertificate, request.Username, null);

            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {newCertificate.Id} Started with reference of {newCertificate.CertificateReference}");

            return newCertificate;
        }
    }
}
