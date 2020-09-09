using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class CreateBatchCertificateHandler : IRequestHandler<CreateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<CreateBatchCertificateHandler> _logger;
        private readonly IStandardService _standardService;

        public CreateBatchCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationQueryRepository, IContactQueryRepository contactQueryRepository, ILogger<CreateBatchCertificateHandler> logger, IStandardService standardService)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _standardService = standardService;
        }

        public async Task<Certificate> Handle(CreateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(CreateBatchCertificateRequest request)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
            var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);

            if(ilr is null)
            {
                _logger.LogWarning($"CreateNewCertificate Did not find ILR for Uln {request.Uln} and StandardCode {request.StandardCode}");
                return null;
            }

            _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            _logger.LogInformation("CreateNewCertificate Before Get Standard from API");
            var standard = await _standardService.GetStandard(ilr.StdCode);

            _logger.LogInformation("CreateNewCertificate Before Get Provider from API");
            var provider = await GetProviderFromUkprn(ilr.UkPrn);

            var certData = CombineCertificateData(request.CertificateData, ilr, standard, provider);

            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate == null)
            {
                _logger.LogInformation("CreateNewCertificate Before create new Certificate");
                 certificate = await _certificateRepository.New(
                    new Certificate()
                    {
                        Uln = request.Uln,
                        StandardCode = request.StandardCode,
                        ProviderUkPrn = ilr.UkPrn,
                        OrganisationId = organisation.Id,
                        CreatedBy = ExternalApiConstants.ApiUserName,
                        CertificateData = JsonConvert.SerializeObject(certData),
                        Status = CertificateStatus.Draft, // NOTE: Web & Staff always creates Draft first
                        CertificateReference = string.Empty,
                        LearnRefNumber = ilr.LearnRefNumber,
                        CreateDay = DateTime.UtcNow.Date
                    });
            }
            else
            {
                _logger.LogInformation("CreateNewCertificate Before resurrecting deleted Certificate");
                certData.EpaDetails.EpaReference = certificate.CertificateReference;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
                certificate.Status = CertificateStatus.Draft;
                await _certificateRepository.Update(certificate, ExternalApiConstants.ApiUserName, CertificateActions.Start);
            }
            
            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {certificate.Id} Started with reference of {certificate.CertificateReference}");

            return await CertificateHelpers.ApplyStatusInformation(_certificateRepository, _contactQueryRepository, certificate);
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            Provider provider = null;
            try
            {
                provider = await _assessmentOrgsApiClient.GetProvider(ukprn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to get Provider from AssessmentOrgsApi. Ukprn: {ukprn}");
            }

            if (provider is null)
            {
                // see if we can get it from Organisation Table
                var org = await _organisationQueryRepository.GetByUkPrn(ukprn);

                if(org != null)
                {
                    provider = new Provider { ProviderName = org.EndPointAssessorName, Ukprn = ukprn };
                }
                else
                {
                    // see whether there are any previous certificates with this ukrpn and a ProviderName....
                    var previousProviderName = await _certificateRepository.GetPreviousProviderName(ukprn);
                    provider = new Provider { ProviderName = previousProviderName ?? "Unknown", Ukprn = ukprn };
                }
            }

            return provider;
        }

        private CertificateData CombineCertificateData(CertificateData data, Ilr ilr, StandardCollation standard, Provider provider)
        {
            var epaDetails = data.EpaDetails ?? new EpaDetails();
            if (epaDetails.Epas is null) epaDetails.Epas = new List<EpaRecord>();

            var epaOutcome = data.OverallGrade == CertificateGrade.Fail ? EpaOutcome.Fail : EpaOutcome.Pass;
            if (data.AchievementDate != null && !epaDetails.Epas.Any(rec => rec.EpaDate == data.AchievementDate.Value && rec.EpaOutcome == epaOutcome))
            {
                var record = new EpaRecord { EpaDate = data.AchievementDate.Value, EpaOutcome = epaOutcome };
                epaDetails.Epas.Add(record);

                var latestRecord = epaDetails.Epas.OrderByDescending(epa => epa.EpaDate).First();
                epaDetails.LatestEpaDate = latestRecord.EpaDate;
                epaDetails.LatestEpaOutcome = latestRecord.EpaOutcome;
            }

            return new CertificateData()
            {
                LearnerGivenNames = ilr.GivenNames,
                LearnerFamilyName = ilr.FamilyName,
                LearningStartDate = ilr.LearnStartDate,
                StandardReference = standard.ReferenceNumber,
                StandardName = standard.Title,   
                StandardLevel = standard.StandardData.Level.GetValueOrDefault(),
                StandardPublicationDate = standard.StandardData.EffectiveFrom,
                FullName = $"{ilr.GivenNames} {ilr.FamilyName}",
                ProviderName = provider.ProviderName,

                ContactName = data.ContactName,
                ContactOrganisation = data.ContactOrganisation,
                Department = data.Department,
                ContactAddLine1 = data.ContactAddLine1,
                ContactAddLine2 = data.ContactAddLine2,
                ContactAddLine3 = data.ContactAddLine3,
                ContactAddLine4 = data.ContactAddLine4,
                ContactPostCode = data.ContactPostCode,
                Registration = data.Registration,
                AchievementDate = data.AchievementDate,
                CourseOption = CertificateHelpers.NormalizeCourseOption(standard, data.CourseOption),
                OverallGrade = CertificateHelpers.NormalizeOverallGrade(data.OverallGrade),

                EpaDetails = epaDetails
            };
        }
    }
}
