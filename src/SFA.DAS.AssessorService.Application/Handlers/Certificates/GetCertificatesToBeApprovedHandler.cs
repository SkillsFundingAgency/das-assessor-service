using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ExternalApis;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesToBeApprovedHandler : IRequestHandler<GetToBeApprovedCertificatesRequest, PaginatedList<CertificateSummaryResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<GetCertificatesToBeApprovedHandler> _logger;

        public GetCertificatesToBeApprovedHandler(ICertificateRepository certificateRepository,
            IRoatpApiClient roatpApiClient,
            IContactQueryRepository contactQueryRepository,
            ILogger<GetCertificatesToBeApprovedHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _roatpApiClient = roatpApiClient;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<PaginatedList<CertificateSummaryResponse>> Handle(GetToBeApprovedCertificatesRequest request,
            CancellationToken cancellationToken)
        {
            var certificates = await _certificateRepository.GetCertificatesForApproval(
                request.PageIndex ?? 1,
                request.PageSize ?? 0,
                request.Status,
                request.PrivatelyFundedStatus);

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var approvals = await MapCertificates(certificates);
            return approvals;
        }

        private async Task<PaginatedList<CertificateSummaryResponse>> MapCertificates(PaginatedList<Certificate> certificates)
        {
            var trainingProviderName = string.Empty;
            var firstName = string.Empty;
            var lastName = string.Empty;
            var reasonForChange = string.Empty;

            var certificateResponses = new List<CertificateSummaryResponse>();

            foreach (var certificate in certificates.Items)
            {
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                var recordedBy = certificate.CreatedBy;

                try
                {
                    if (certificateData.ProviderName == null)
                    {
                        var provider = (await _roatpApiClient.SearchOrganisationByUkprn(certificate.ProviderUkPrn)).FirstOrDefault();
                        if (provider == null)
                        {
                            throw new EntityNotFoundException($"Training provider {certificate.ProviderUkPrn} not found", null);
                        }
                            
                        trainingProviderName = provider?.ProviderName;
                        await _certificateRepository.UpdateProviderName(certificate.Id, trainingProviderName);
                    }
                    else
                    {
                        trainingProviderName = certificateData.ProviderName;
                    }

                    firstName = certificateData.LearnerGivenNames;
                    lastName = certificateData.LearnerFamilyName;
                    reasonForChange = certificate.CertificateLogs
                        .OrderByDescending(q => q.EventTime)
                        .FirstOrDefault(certificateLog =>
                            certificateLog.Status == Domain.Consts.CertificateStatus.Draft && certificateLog.Action == Domain.Consts.CertificateStatus.Rejected)?.ReasonForChange;
                }
                catch (EntityNotFoundException)
                {
                    _logger.LogInformation(
                        $"Cannot find training provider for ukprn {certificate.Organisation.EndPointAssessorUkprn.Value}");
                }

                certificateResponses.Add(new CertificateSummaryResponse
                {
                    CertificateReference = certificate.CertificateReference,
                    Uln = certificate.Uln,
                    CreatedDay = certificate.CreateDay,
                    UpdatedAt = certificate.UpdatedAt,
                    ContactOrganisation = certificateData.ContactOrganisation,
                    ContactName = certificateData.ContactName,
                    TrainingProvider = trainingProviderName,
                    RecordedBy = recordedBy,
                    CourseOption = certificateData.CourseOption,
                    FullName = certificateData.FullName,
                    OverallGrade = certificateData.OverallGrade,
                    StandardReference = certificateData.StandardReference,
                    StandardName = certificateData.StandardName,
                    Level = certificateData.StandardLevel,
                    AchievementDate = certificateData.AchievementDate,
                    LearningStartDate = certificateData.LearningStartDate,
                    ContactAddLine1 = certificateData.ContactAddLine1,
                    ContactAddLine2 = certificateData.ContactAddLine2,
                    ContactAddLine3 = certificateData.ContactAddLine3,
                    ContactAddLine4 = certificateData.ContactAddLine4,
                    ContactPostCode = certificateData.ContactPostCode,
                    Status = certificate.Status,
                    PrivatelyFundedStatus = certificate.PrivatelyFundedStatus,
                    FirstName = firstName,
                    LastName = lastName,
                    Ukprn = certificate.ProviderUkPrn,
                    StandardCode = certificate.StandardCode,
                    EpaoId = certificate.Organisation?.EndPointAssessorOrganisationId,
                    EpaoName = certificate.Organisation?.EndPointAssessorName,
                    CertificateId = certificate.Id,
                    ReasonForChange = reasonForChange
                });
            }

            var responses = certificateResponses.ToList();
            foreach (var response in responses)
            {
                var contact = await _contactQueryRepository.GetContact(response.RecordedBy);
                response.RecordedBy = contact?.DisplayName;
                response.RecordedByEmail = contact?.Email;
            }

            var paginatedList = new PaginatedList<CertificateSummaryResponse>(responses,
                certificates.TotalRecordCount,
                certificates.PageIndex,
                certificates.PageSize
            );

            return paginatedList;
        }
    }
}