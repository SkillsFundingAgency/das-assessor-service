using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
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
            if(certificate.Status == CertificateStatus.Deleted)
            {
                // Rehydrate cert data when the certificate is deleted
                certificate.CertificateData = new CertificateData();
            }

            certificate = await PopulateCertificateData(certificate, request, organisation);
            
            // If the certificate was a fail, reset back to draft and reset achievement date and grade
            if (certificate.Status == CertificateStatus.Submitted && certificate.CertificateData.OverallGrade == CertificateGrade.Fail)
            {
                certificate.CertificateData.AchievementDate = null;
                certificate.CertificateData.OverallGrade = null;
                certificate.Status = CertificateStatus.Draft;
                certificate = await _certificateRepository.UpdateStandardCertificate(certificate, request.Username, CertificateActions.Restart, updateLog: true);
            }
            else
            {
                certificate = await _certificateRepository.UpdateStandardCertificate(certificate, request.Username, null);
            }
            
            return certificate;
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request, Organisation organisation)
        {
            var certificate = new Certificate
            {
                CertificateData = new CertificateData
                {
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
                },
                Uln = request.Uln,
                StandardCode = request.StandardCode,
                Status = CertificateStatus.Draft,
                CreatedBy = request.Username,
                CertificateReference = string.Empty,
                CreateDay = DateTime.UtcNow.Date
            };

            certificate = await PopulateCertificateData(certificate, request, organisation);
            var newCertificate = await _certificateRepository.NewStandardCertificate(certificate);

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
        private async Task<Certificate> PopulateCertificateData(Certificate certificate, StartCertificateRequest request, Organisation organisation)
        {
            var learner = await _learnerRepository.Get(request.Uln, request.StandardCode);
            
            var provider = await GetProviderFromUkprn(learner.UkPrn);

            if ((learner.GivenNames.ToLower() == learner.GivenNames) || (learner.GivenNames.ToUpper() == learner.GivenNames))
            {
                certificate.CertificateData.LearnerGivenNames = _certificateNameCapitalisationService.ProperCase(learner.GivenNames);
            }
            else
            {
                certificate.CertificateData.LearnerGivenNames = learner.GivenNames;
            }

            if ((learner.FamilyName.ToLower() == learner.FamilyName) || (learner.FamilyName.ToUpper() == learner.FamilyName))
            {
                certificate.CertificateData.LearnerFamilyName = _certificateNameCapitalisationService.ProperCase(learner.FamilyName, true);
            }
            else
            {
                certificate.CertificateData.LearnerFamilyName = learner.FamilyName;
            }

            certificate.CertificateData.EmployerAccountId = learner.EmployerAccountId;
            certificate.CertificateData.EmployerName = learner.EmployerName;

            certificate.CertificateData.LearningStartDate = learner.LearnStartDate;
            certificate.CertificateData.FullName = $"{certificate.CertificateData.LearnerGivenNames} {certificate.CertificateData.LearnerFamilyName}";
            certificate.CertificateData.ProviderName = provider.Name;
            certificate.CertificateData.CoronationEmblem = await _standardRepository.GetCoronationEmblemForStandardReferenceAndVersion(learner.StandardReference, learner.Version);

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

                certificate.CertificateData.StandardName = standardVersion.Title;
                certificate.CertificateData.StandardReference = standardVersion.IfateReferenceNumber;
                certificate.CertificateData.StandardLevel = standardVersion.Level;
                certificate.CertificateData.StandardPublicationDate = standardVersion.VersionApprovedForDelivery;
                certificate.CertificateData.Version = standardVersion.Version;

                if (!string.IsNullOrWhiteSpace(request.CourseOption))
                {
                    certificate.CertificateData.CourseOption = request.CourseOption;
                }

                certificate.StandardUId = standardVersion.StandardUId;
            }
            else
            {
                _logger.LogInformation($"Populating certificate data for standard versions of StdCode:{learner.StdCode}");
                var standardVersions = await _standardService.GetStandardVersionsByLarsCode(learner.StdCode);
                var latestStandardVersion = standardVersions.OrderByDescending(s => s.VersionMajor).ThenByDescending(t => t.VersionMinor).First();
                
                certificate.CertificateData.StandardName = latestStandardVersion.Title;
                certificate.CertificateData.StandardReference = latestStandardVersion.IfateReferenceNumber;
            }

            return certificate;
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            return await _providersRepository.GetProvider(ukprn);
        }
    }
}