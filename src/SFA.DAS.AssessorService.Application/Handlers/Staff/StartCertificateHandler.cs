using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StartCertificateHandler : IRequestHandler<StartCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IProvidersRepository _providersRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<StartCertificateHandler> _logger;
        private readonly IStandardService _standardService;
        private readonly ICertificateNameCapitalisationService _certificateNameCapitalisationService;
        private readonly int PrivateFundingModelNumber = 99;

        public StartCertificateHandler(ICertificateRepository certificateRepository, ILearnerRepository learnerRepository, IProvidersRepository providersRepository,
            IOrganisationQueryRepository organisationQueryRepository, IStandardRepository standardRepository, ILogger<StartCertificateHandler> logger, IStandardService standardService, ICertificateNameCapitalisationService certificateNameCapitalisationService)
        {
            _certificateRepository = certificateRepository;
            _learnerRepository = learnerRepository;
            _providersRepository = providersRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _standardRepository = standardRepository;
            _logger = logger;
            _standardService = standardService;
            _certificateNameCapitalisationService = certificateNameCapitalisationService;
        }

        public async Task<Certificate> Handle(StartCertificateRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting new certificate for EPAO UkPrn:{request.UkPrn}");
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
            _logger.LogDebug($"Updating existing certificate for Uln:{request.Uln} StandardCode:{request.StandardCode} StandardUId{request.StandardUId}");
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
                certificate = await _certificateRepository.Update(certificate, request.Username, null);
            }
            
            return certificate;
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request, Organisation organisation)
        {
            _logger.LogDebug($"Creating new certificate for Uln:{request.Uln} StandardCode:{request.StandardCode} StandardUId{request.StandardUId}");

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
            var newCertificate = await _certificateRepository.New(certificate);

            _logger.LogDebug($"Created new certificate with Id:{newCertificate.Id} CertificateReference:{newCertificate.CertificateReference}");
            
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
            _logger.LogDebug($"Populating certificate data for Uln:{request.Uln} StandardCode:{request.StandardCode}");
            var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);
            
            _logger.LogDebug($"Populating certificate data with provider UkPrn:{learner.UkPrn}");
            var provider = await GetProviderFromUkprn(learner.UkPrn);

            if ((learner.GivenNames.ToLower() == learner.GivenNames) || (learner.GivenNames.ToUpper() == learner.GivenNames))
            {
                certData.LearnerGivenNames = _certificateNameCapitalisationService.ProperCase(learner.GivenNames);
            }
            else
            {
                certData.LearnerGivenNames = learner.GivenNames;
            }

            if ((learner.FamilyName.ToLower() == learner.FamilyName) || (learner.FamilyName.ToUpper() == learner.FamilyName))
            {
                certData.LearnerFamilyName = _certificateNameCapitalisationService.ProperCase(learner.FamilyName, true);
            }
            else
            {
                certData.LearnerFamilyName = learner.FamilyName;
            }

            certData.EmployerAccountId = learner.EmployerAccountId;
            certData.EmployerName = learner.EmployerName;

            certData.LearningStartDate = learner.LearnStartDate;
            certData.FullName = $"{certData.LearnerGivenNames} {certData.LearnerFamilyName}";
            certData.ProviderName = provider.Name;
            certData.CoronationEmblem = await _standardRepository.GetCoronationEmblemForStandardReferenceAndVersion(learner.StandardReference, learner.Version);

            certificate.ProviderUkPrn = learner.UkPrn;
            certificate.OrganisationId = organisation.Id;
            certificate.LearnRefNumber = learner.LearnRefNumber;
            certificate.IsPrivatelyFunded = learner?.FundingModel == PrivateFundingModelNumber;

            if (!string.IsNullOrWhiteSpace(request.StandardUId))
            {
                _logger.LogInformation($"Populating certificate data for StandardUId:{request.StandardUId}");
                
                var standardVersion = await _standardService.GetStandardVersionById(request.StandardUId);
                if (standardVersion == null)
                {
                    throw new InvalidOperationException($"StandardUId:{request.StandardUId} not found, unable to populate certificate data");
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
                _logger.LogDebug($"Populating certificate data for standard versions of StdCode:{learner.StdCode}");
                var standardVersions = await _standardService.GetStandardVersionsByLarsCode(learner.StdCode);
                var latestStandardVersion = standardVersions.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First();
                
                certData.StandardName = latestStandardVersion.Title;
                certData.StandardReference = latestStandardVersion.IfateReferenceNumber;
            }

            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            return await _providersRepository.GetProvider(ukprn);
        }
    }
}