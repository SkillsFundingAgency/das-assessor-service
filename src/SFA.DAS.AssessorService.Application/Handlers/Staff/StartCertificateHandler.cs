using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ProviderRegister;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StartCertificateHandler : IRequestHandler<StartCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;
        private readonly IStandardService _standardService;
        private readonly int PrivateFundingModelNumber = 99;

        public StartCertificateHandler(ICertificateRepository certificateRepository, ILearnerRepository learnerRepository, IRoatpApiClient roatpApiClient,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger, IStandardService standardService)
        {
            _certificateRepository = certificateRepository;
            _learnerRepository = learnerRepository;
            _roatpApiClient = roatpApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
            _standardService = standardService;
        }

        public async Task<Certificate> Handle(StartCertificateRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"StartCertificateRequest for EPAO Ukprn: {request.UkPrn}");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate == null)
            {
                certificate = await CreateNewCertificate(request, organisation);
            }
            else
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);

                // when re-using an existing certificate ensure that the organistion matches the Ukprn for
                // the latest organisation which is assessing the learner
                certificate.OrganisationId = organisation.Id;

                if (certificate.Status == CertificateStatus.Deleted && learner != null)
                {
                    _logger.LogInformation($"Recreating deleted certificate for ULN: {certificate.Uln}, Standard: {certificate.StandardCode}");

                    certData.LearnerGivenNames = learner.GivenNames;
                    certData.LearnerFamilyName = learner.FamilyName;
                    certData.LearningStartDate = learner.LearnStartDate;
                    certData.FullName = $"{learner.GivenNames} {learner.FamilyName}";
                    certificate.CertificateData = JsonConvert.SerializeObject(certData);
                    certificate.IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber;
                    await _certificateRepository.Update(certificate, request.Username, null);
                }
                else if (certificate.Status == CertificateStatus.Submitted && certData.OverallGrade == CertificateGrade.Fail)
                {
                    _logger.LogInformation($"Restarting apprenticeship for ULN: {certificate.Uln}, Standard: {certificate.StandardCode}");

                    certData.AchievementDate = null;
                    certData.OverallGrade = null;
                    certificate.IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber;
                    certificate.CertificateData = JsonConvert.SerializeObject(certData);
                    certificate.Status = CertificateStatus.Draft;

                    await _certificateRepository.Update(certificate, request.Username, CertificateActions.Restart, updateLog: true);
                }
                else if (learner != null)
                {
                    // If Learner not null, and certificate exists, reset privately funded
                    // In case it's an old draft privately funded with a new Learner record 
                    certificate.IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber;
                    await _certificateRepository.Update(certificate, request.Username, null);
                }
            }

            return certificate;
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request, Organisation organisation)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Learner from db");
            var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);
            _logger.LogInformation("CreateNewCertificate Before Get Provider from API");
            var provider = await GetProviderFromUkprn(learner.UkPrn);

            var certData = new CertificateData()
            {
                LearnerGivenNames = learner.GivenNames,
                LearnerFamilyName = learner.FamilyName,
                LearningStartDate = learner.LearnStartDate,
                FullName = $"{learner.GivenNames} {learner.FamilyName}",
                ProviderName = provider.ProviderName,
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
            };

            var certificate = new Certificate()
            {
                Uln = request.Uln,
                StandardCode = request.StandardCode,
                ProviderUkPrn = learner.UkPrn,
                OrganisationId = organisation.Id,
                CreatedBy = request.Username,
                CertificateData = JsonConvert.SerializeObject(certData),
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                LearnRefNumber = learner.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber,
            };

            // Only one StandardUid Available, fill out fields
            if (!string.IsNullOrWhiteSpace(request.StandardUId))
            {
                _logger.LogInformation("CreateNewCertificate Before Get Single StandardVersion from API");
                var standardVersion = await _standardService.GetStandardVersionById(request.StandardUId);

                if (standardVersion == null)
                {
                    throw new InvalidOperationException("StandardUId Provided not recognised, unable to start certificate request");
                }

                certData.StandardName = standardVersion.Title;
                certData.StandardReference = standardVersion.IfateReferenceNumber;
                certData.StandardLevel = standardVersion.Level;
                certData.StandardPublicationDate = standardVersion.EffectiveFrom;
                certData.Version = standardVersion.Version;

                if (!string.IsNullOrWhiteSpace(request.CourseOption))
                {
                    certData.CourseOption = request.CourseOption;
                }

                certificate.StandardUId = standardVersion.StandardUId;
            }
            else
            {
                _logger.LogInformation("CreateNewCertificate Before Get StandardVersions from API");
                var standardVersions = await _standardService.GetStandardVersionsByLarsCode(learner.StdCode);

                certData.StandardName = standardVersions.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First().Title;
            }

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            _logger.LogInformation("CreateNewCertificate Before create new Certificate");
            var newCertificate = await _certificateRepository.New(certificate);

            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {newCertificate.Id} Started with reference of {newCertificate.CertificateReference}");

            return newCertificate;
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            Provider provider;
            OrganisationSearchResult searchResult = null;
            try
            {
                searchResult = await _roatpApiClient.GetOrganisationByUkprn(ukprn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to get Provider from AssessmentOrgsApi. Ukprn: {ukprn}");
            }

            if (searchResult is null)
            {
                // see if we can get it from Organisation Table
                var org = await _organisationQueryRepository.GetByUkPrn(ukprn);

                if (org != null)
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
            else
            {
                provider = new Provider { ProviderName = searchResult.ProviderName, Ukprn = ukprn };
            }

            return provider;
        }
    }

}