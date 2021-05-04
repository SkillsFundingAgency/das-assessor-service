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
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StartCertificateHandler : IRequestHandler<StartCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly ILogger<StartCertificateHandler> _logger;
        private readonly IStandardService _standardService;
        private readonly int PrivateFundingModelNumber = 99;

        public StartCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IRoatpApiClient roatpApiClient,
            IOrganisationQueryRepository organisationQueryRepository, ILogger<StartCertificateHandler> logger, IStandardService standardService)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _roatpApiClient = roatpApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _logger = logger;
            _standardService = standardService;
        }

        public async Task<Certificate> Handle(StartCertificateRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate == null)
                certificate = await CreateNewCertificate(request);
            else
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

                if (certificate.Status == CertificateStatus.Deleted)
                {
                    _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
                    var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
                    if (ilr != null)
                    {
                        certData.LearnerGivenNames = ilr.GivenNames;
                        certData.LearnerFamilyName = ilr.FamilyName;
                        certData.LearningStartDate = ilr.LearnStartDate;
                        certData.FullName = $"{ilr.GivenNames} {ilr.FamilyName}";
                        certificate.CertificateData = JsonConvert.SerializeObject(certData);

                        certificate.IsPrivatelyFunded = ilr?.FundingModel == PrivateFundingModelNumber;
                        await _certificateRepository.Update(certificate, request.Username, null);
                    }
                }
                else if (certificate.Status == CertificateStatus.Submitted && certData.OverallGrade == CertificateGrade.Fail)
                {
                    _logger.LogInformation($"Starting retake of apprenticeship for ULN: {certificate.Uln}, Standard: {certificate.StandardCode}");

                    certificate.Status = CertificateStatus.Draft;

                    await _certificateRepository.Update(certificate, request.Username, CertificateActions.StartRetake, updateLog: true, "Retake failed apprenticeship");
                }
            }

            return certificate;
        }

        private async Task<Certificate> CreateNewCertificate(StartCertificateRequest request)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
            var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
            _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);
            _logger.LogInformation("CreateNewCertificate Before Get Provider from API");
            var provider = await GetProviderFromUkprn(ilr.UkPrn);

            var certData = new CertificateData()
            {
                LearnerGivenNames = ilr.GivenNames,
                LearnerFamilyName = ilr.FamilyName,
                LearningStartDate = ilr.LearnStartDate,
                FullName = $"{ilr.GivenNames} {ilr.FamilyName}",
                ProviderName = provider.ProviderName,
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
            };

            var certificate = new Certificate()
            {
                Uln = request.Uln,
                StandardCode = request.StandardCode,
                ProviderUkPrn = ilr.UkPrn,
                OrganisationId = organisation.Id,
                CreatedBy = request.Username,
                CertificateData = JsonConvert.SerializeObject(certData),
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                LearnRefNumber = ilr.LearnRefNumber,
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = ilr?.FundingModel == PrivateFundingModelNumber,
            };

            // Only one StandardUid Available, fill out fields
            if (!string.IsNullOrWhiteSpace(request.StandardUId))
            {
                _logger.LogInformation("CreateNewCertificate Before Get Single StandardVersion from API");
                var standardVersion = await _standardService.GetStandardVersionById(request.StandardUId);

                if(standardVersion == null)
                {
                    throw new InvalidOperationException("StandardUId Provided not recognised, unable to start certificate request");
                }

                certData.StandardName = standardVersion.Title;
                certData.StandardReference = standardVersion.IfateReferenceNumber;
                certData.StandardLevel = standardVersion.Level;
                certData.StandardPublicationDate = standardVersion.EffectiveFrom;
                certData.Version = standardVersion.Version.GetValueOrDefault(1).ToString("#.0");

                if(!string.IsNullOrWhiteSpace(request.CourseOption))
                {
                    certData.CourseOption = request.CourseOption;
                }

                certificate.StandardUId = standardVersion.StandardUId;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
            }
            else
            {
                _logger.LogInformation("CreateNewCertificate Before Get StandardVersions from API");
                var standardVersions = await _standardService.GetStandardVersionsByLarsCode(ilr.StdCode);

                certData.StandardName = standardVersions.OrderByDescending(s => s.Version).First().Title;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
            }

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