using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class CreateBatchCertificateHandler : IRequestHandler<CreateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<CreateBatchCertificateHandler> _logger;
        private readonly IStandardService _standardService;
        private readonly IProvidersRepository _providersRepository;

        public CreateBatchCertificateHandler(ICertificateRepository certificateRepository, ILearnerRepository learnerRepository,
            IOrganisationQueryRepository organisationQueryRepository, IContactQueryRepository contactQueryRepository, 
            ILogger<CreateBatchCertificateHandler> logger, IStandardService standardService, IProvidersRepository providersRepository)
        {
            _certificateRepository = certificateRepository;
            _learnerRepository = learnerRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _standardService = standardService;
            _providersRepository = providersRepository;
        }

        public async Task<Certificate> Handle(CreateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(CreateBatchCertificateRequest request)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Learner from db");
            var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);

            if (learner is null)
            {
                _logger.LogWarning($"CreateNewCertificate Did not find Learner for Uln {request.Uln} and StandardCode {request.StandardCode}");
                return null;
            }

            _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            _logger.LogInformation("CreateNewCertificate Before Get Standard from API");
            var standard = await _standardService.GetStandardVersionById(request.StandardUId);

            var coronationEmblem = await _standardService.GetCoronationEmblemForStandardReferenceAndVersion(request.StandardReference, request.CertificateData.Version);

            _logger.LogInformation("CreateNewCertificate Before Get Provider from API");
            var provider = await GetProviderFromUkprn(learner.UkPrn);

            _logger.LogInformation("CreateNewCertificate Before Get StandardOptions from API");
            var options = await _standardService.GetStandardOptionsByStandardId(request.StandardUId);

            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            var certificateData = CombineCertificateData(request.CertificateData, coronationEmblem, learner, standard, provider, options, certificate);

            if (certificate == null)
            {
                _logger.LogInformation("CreateNewCertificate Before create new Certificate");
                certificate = await _certificateRepository.NewStandardCertificate(
                   new Certificate()
                   {
                       Uln = request.Uln,
                       StandardCode = request.StandardCode,
                       StandardUId = request.StandardUId,
                       ProviderUkPrn = learner.UkPrn,
                       OrganisationId = organisation.Id,
                       CreatedBy = ExternalApiConstants.ApiUserName,
                       CertificateData = certificateData,
                       Status = CertificateStatus.Draft, // NOTE: Web & Staff always creates Draft first
                       CertificateReference = string.Empty,
                       LearnRefNumber = learner.LearnRefNumber,
                       CreateDay = DateTime.UtcNow.Date
                   });
            }
            else
            {
                _logger.LogInformation("CreateNewCertificate Before resurrecting deleted Certificate");
                certificateData.EpaDetails.EpaReference = certificate.CertificateReference;
                certificate.CertificateData = certificateData;
                certificate.StandardUId = request.StandardUId;
                certificate.Status = CertificateStatus.Draft;
                await _certificateRepository.UpdateStandardCertificate(certificate, ExternalApiConstants.ApiUserName, CertificateActions.Start);
            }

            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {certificate.Id} Started with reference of {certificate.CertificateReference}");

            return await CertificateHelpers.ApplyStatusInformation(_certificateRepository, _contactQueryRepository, certificate);
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            return await _providersRepository.GetProvider(ukprn);
        }

        private CertificateData CombineCertificateData(CertificateData data, bool coronationEmblem, Domain.Entities.Learner learner, Standard standard, Provider provider, StandardOptions options, Certificate certificate)
        {
            var epaDetails = new EpaDetails();
            if (certificate != null)
            {
                if (certificate.CertificateData.EpaDetails != null)
                {
                    epaDetails = certificate.CertificateData.EpaDetails;
                }
            }
            
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
                LearnerGivenNames = learner.GivenNames,
                LearnerFamilyName = learner.FamilyName,
                LearningStartDate = learner.LearnStartDate,
                StandardReference = standard.IfateReferenceNumber,
                StandardName = standard.Title,
                StandardLevel = standard.Level,
                StandardPublicationDate = standard.EffectiveFrom,
                FullName = $"{learner.GivenNames} {learner.FamilyName}",
                ProviderName = provider.Name,

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
                CourseOption = CertificateHelpers.NormalizeCourseOption(options, data.CourseOption),
                OverallGrade = CertificateHelpers.NormalizeOverallGrade(data.OverallGrade),
                Version = data.Version,
                CoronationEmblem = coronationEmblem,

                EpaDetails = epaDetails
            };
        }
    }
}
