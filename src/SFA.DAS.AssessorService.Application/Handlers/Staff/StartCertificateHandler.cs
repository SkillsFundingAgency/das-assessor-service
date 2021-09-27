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
                certificate = await UpdateExistingCertificate(request, organisation, certificate);
            }

            return certificate;
        }

        private async Task<Certificate> UpdateExistingCertificate(StartCertificateRequest request, Organisation organisation, Certificate certificate)
        {
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            if(certificate.Status == CertificateStatus.Deleted)
            {
                // Rehydrate cert data when the certificate is deleted
                certData = new CertificateData();
            };

            certificate = await PopulateCertificateData(certificate, certData, request, organisation);
            
            // If the certificate was a fail, reset back to draft and reset achievement date and grade
            if (certificate.Status == CertificateStatus.Submitted && certData.OverallGrade == CertificateGrade.Fail)
            {
                certData.AchievementDate = null;
                certData.OverallGrade = null;
                certificate.Status = CertificateStatus.Draft;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
                certificate = await _certificateRepository.Update(certificate, request.Username, CertificateActions.Restart, updateLog: true);
            }
            else
            {
                certificate = await PopulateCertificateData(certificate, certData, request, organisation);
                certificate = await _certificateRepository.Update(certificate, request.Username, null);
            }
            
            return certificate;
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request, Organisation organisation)
        {
            var certificate = new Certificate();
            var certificateData = new CertificateData
            {
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
            };

            certificate.Uln = request.Uln;
            certificate.StandardCode = request.StandardCode;
            certificate.Status = CertificateStatus.Draft;
            certificate.CreatedBy = request.Username;
            certificate.CertificateReference = string.Empty;
            certificate.CreateDay = DateTime.UtcNow.Date;

            certificate = await PopulateCertificateData(certificate, certificateData, request, organisation);

            _logger.LogInformation("CreateNewCertificate Before create new Certificate");
            var newCertificate = await _certificateRepository.New(certificate);

            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {newCertificate.Id} Started with reference of {newCertificate.CertificateReference}");
            return newCertificate;
        }

        /// <summary>
        /// This method can be used to create a new certificate where learner and provider information is populated
        /// or to populate an existing certificate when resuming the journey.
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="certData"></param>
        /// <param name="request"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        private async Task<Certificate> PopulateCertificateData(Certificate certificate, CertificateData certData, StartCertificateRequest request, Organisation organisation)
        {
            _logger.LogInformation("PopulateCertificateData Before Get Learner from db");
            var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);
            _logger.LogInformation("PopulateCertificateData Before Get Provider from API");
            var provider = await GetProviderFromUkprn(learner.UkPrn);

            certData.LearnerGivenNames = learner.GivenNames;
            certData.LearnerFamilyName = learner.FamilyName;
            certData.LearningStartDate = learner.LearnStartDate;
            certData.FullName = $"{learner.GivenNames} {learner.FamilyName}";
            certData.ProviderName = provider.ProviderName;
                        
            certificate.ProviderUkPrn = learner.UkPrn;
            certificate.OrganisationId = organisation.Id;
            certificate.LearnRefNumber = learner.LearnRefNumber;
            certificate.IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber;

            // Only one StandardUid Available, fill out fields
            if (!string.IsNullOrWhiteSpace(request.StandardUId))
            {
                _logger.LogInformation("PopulateCertificateData Before Get Single StandardVersion from API");
                var standardVersion = await _standardService.GetStandardVersionById(request.StandardUId);

                if (standardVersion == null)
                {
                    throw new InvalidOperationException("StandardUId Provided not recognised, unable to populate certificate data");
                }

                certData.StandardName = standardVersion.Title;
                certData.StandardReference = standardVersion.IfateReferenceNumber;
                certData.StandardLevel = standardVersion.Level;
                certData.StandardPublicationDate = standardVersion.VersionApprovedForDelivery;
                certData.Version = standardVersion.Version;

                if (!string.IsNullOrWhiteSpace(request.CourseOption))
                {
                    certData.CourseOption = request.CourseOption;
                }

                certificate.StandardUId = standardVersion.StandardUId;
            }
            else
            {
                _logger.LogInformation("PopulateCertificateData Before Get StandardVersions from API");
                var standardVersions = await _standardService.GetStandardVersionsByLarsCode(learner.StdCode);

                var latestStandardVersion = standardVersions.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First();
                certData.StandardName = latestStandardVersion.Title;
                certData.StandardReference = latestStandardVersion.IfateReferenceNumber;
            }

            // Populate Cert Data at end
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
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