using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class CreateBatchCertificateHandler : IRequestHandler<CreateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<CreateBatchCertificateHandler> _logger;

        public CreateBatchCertificateHandler(ICertificateRepository certificateRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationQueryRepository, IContactQueryRepository contactQueryRepository, ILogger<CreateBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(CreateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewCertificate(request);
        }

        private async Task<Certificate> CreateNewCertificate(CreateBatchCertificateRequest request)
        {
            _logger.LogInformation("CreateNewCertificate Before Get Contact from API");
            var contact = await GetContactFromEmailAddress(request.Email);
            _logger.LogInformation("CreateNewCertificate Before Get Ilr from db");
            var ilr = await _ilrRepository.Get(request.Uln, request.StandardCode);
            _logger.LogInformation("CreateNewCertificate Before Get Organisation from db");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);
            _logger.LogInformation("CreateNewCertificate Before Get Standard from API");
            var standard = await _assessmentOrgsApiClient.GetStandard(ilr.StdCode);
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
                        CreatedBy = contact.Username,
                        CertificateData = JsonConvert.SerializeObject(certData),
                        Status = CertificateStatus.Draft,
                        CertificateReference = "",
                        LearnRefNumber = ilr.LearnRefNumber
                    });

                certificate.CertificateReference = certificate.CertificateReferenceId.ToString().PadLeft(8, '0');
            }
            else
            {
                _logger.LogInformation("CreateNewCertificate Before resurrecting deleted Certificate");
                certificate.Status = CertificateStatus.Draft;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
            }

            _logger.LogInformation("CreateNewCertificate Before Update Cert in db");
            await _certificateRepository.Update(certificate, contact.Username, null);

            _logger.LogInformation(LoggingConstants.CertificateStarted);
            _logger.LogInformation($"Certificate with ID: {certificate.Id} Started with reference of {certificate.CertificateReference}");

            return await ApplyStatusInformation(certificate);
        }

        private async Task<Provider> GetProviderFromUkprn(int ukprn)
        {
            Provider provider;
            try
            {
                provider = await _assessmentOrgsApiClient.GetProvider(ukprn);
            }
            catch (Exception)
            {
                // see whether there are any previous certificates with this ukrpn and a ProviderName....
                var previousProviderName = await _certificateRepository.GetPreviousProviderName(ukprn);
                provider = previousProviderName != null
                    ? new Provider { ProviderName = previousProviderName }
                    : new Provider { ProviderName = "Unknown" };
            }

            return provider;
        }

        private async Task<Contact> GetContactFromEmailAddress(string email)
        {
            Contact contact = await _contactQueryRepository.GetContactFromEmailAddress(email);

            if( contact == null)
            {
                contact = new Contact { Username = email, Email = email };
            }

            return contact;
        }

        private CertificateData CombineCertificateData(CertificateData data, Ilr ilr, Standard standard, Provider provider)
        {
            return new CertificateData()
            {
                LearnerGivenNames = ilr.GivenNames,
                LearnerFamilyName = ilr.FamilyName,
                StandardName = standard.Title,
                LearningStartDate = ilr.LearnStartDate,
                StandardLevel = standard.Level,
                StandardPublicationDate = standard.EffectiveFrom.Value,
                FullName = $"{ilr.GivenNames} {ilr.FamilyName}",
                ProviderName = provider.ProviderName,

                ContactName = data.ContactName,
                ContactOrganisation = data.ContactOrganisation,
                ContactAddLine1 = data.ContactAddLine1,
                ContactAddLine2 = data.ContactAddLine2,
                ContactAddLine3 = data.ContactAddLine3,
                ContactAddLine4 = data.ContactAddLine4,
                ContactPostCode = data.ContactPostCode,
                Registration = data.Registration,
                AchievementDate = data.AchievementDate,
                CourseOption = data.CourseOption,
                OverallGrade = data.OverallGrade,
                Department = data.Department
            };
        }

        private async Task<Certificate> ApplyStatusInformation(Certificate certificate)
        {  
            var certificateLogs = await _certificateRepository.GetCertificateLogsFor(certificate.Id);

            var createdLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Draft);
            if (createdLogEntry != null)
            {
                var createdContact = await _contactQueryRepository.GetContact(createdLogEntry.Username);
                certificate.CreatedAt = createdLogEntry.EventTime.UtcToTimeZoneTime();
                certificate.CreatedBy = createdContact != null ? createdContact.DisplayName : createdLogEntry.Username;
            }

            var submittedLogEntry = certificateLogs?.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);

            // NOTE: THIS IS A DATA FRIG FOR EXTERNAL API AS WE NEED SUBMITTED INFORMATION!
            if (submittedLogEntry != null)
            {
                var submittedContact = await _contactQueryRepository.GetContact(submittedLogEntry.Username);
                certificate.UpdatedAt = submittedLogEntry.EventTime.UtcToTimeZoneTime();
                certificate.UpdatedBy = submittedContact != null ? submittedContact.DisplayName : createdLogEntry.Username;
            }
            else
            {
                certificate.UpdatedAt = null;
                certificate.UpdatedBy = null;
            }

            return certificate;
        }
    }
}
