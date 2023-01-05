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
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesHistoryHandler : IRequestHandler<GetCertificateHistoryRequest, PaginatedList<CertificateSummaryResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;
        private List<string> _ignoreStatuses;

        public GetCertificatesHistoryHandler(ICertificateRepository certificateRepository,
            IRoatpApiClient roatpApiClient,
            IContactQueryRepository contactQueryRepository,
            ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _roatpApiClient = roatpApiClient;
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _ignoreStatuses = new List<string>
            {
                Domain.Consts.CertificateStatus.Deleted,
                Domain.Consts.CertificateStatus.Draft,
            };
        }

        public async Task<PaginatedList<CertificateSummaryResponse>> Handle(GetCertificateHistoryRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 100;

            var searchResult = await _certificateRepository.GetCertificateHistory(
                request.EndPointAssessorOrganisationId,
                request.PageIndex ?? 1,
                pageSize,
                request.SearchTerm,
                request.SortColumn,
                request.SortDescending,
                _ignoreStatuses);

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var certificateHistoryResponses = MapCertificates(searchResult);

            return await certificateHistoryResponses;
        }

        private async Task<PaginatedList<CertificateSummaryResponse>> MapCertificates(PaginatedList<CertificateHistoryModel> certificates)
        {
            var certificateResponses = certificates?.Items.Select(
                certificate =>
                {
                    var latestCertificateStatusDate = (new[] { certificate.UpdatedAt, certificate.CreatedAt }.Max()).Value;

                    var recordedBy = certificate.CertificateLogs
                            .OrderByDescending(q => q.EventTime)
                            .FirstOrDefault(certificateLog =>
                                certificateLog.Action == Domain.Consts.CertificateActions.Submit)?.Username;

                    var printStatusAt = certificate?.StatusAt;
                    var printReasonForChange = certificate?.ReasonForChange;

                    var trainingProviderName = string.Empty;
                    try
                    {
                        if (certificate.ProviderName == null)
                        {
                            var provider = _roatpApiClient
                                .GetOrganisationByUkprn(certificate.ProviderUkPrn).Result;

                            if (provider == null)
                            {
                                throw new EntityNotFoundException($"Provider {certificate.ProviderUkPrn} not found", null);
                            }

                            trainingProviderName = provider.ProviderName;
                            _certificateRepository.UpdateProviderName(certificate.Id, trainingProviderName).GetAwaiter().GetResult();
                        }
                        else
                        {
                            trainingProviderName = certificate.ProviderName;
                        }
                    }
                    catch (EntityNotFoundException)
                    {
                        if (certificate.EndPointAssessorUkprn != null)
                            _logger.LogInformation(
                                $"Cannot find training provider for ukprn {certificate.EndPointAssessorUkprn.Value}");
                    }

                    return new CertificateSummaryResponse
                    {
                        CertificateReference = certificate.CertificateReference,
                        Uln = certificate.Uln,
                        CreatedAt = certificate.CreatedAt,
                        CreatedDay = certificate.CreateDay,
                        UpdatedAt = certificate.UpdatedAt,
                        PrintStatusAt = printStatusAt,
                        ContactOrganisation = certificate.ContactOrganisation,
                        ContactName = certificate.ContactName,
                        TrainingProvider = trainingProviderName,
                        RecordedBy = recordedBy,
                        CourseOption = certificate.CourseOption,
                        FullName = certificate.FullName,
                        OverallGrade = certificate.OverallGrade,
                        StandardReference = certificate.StandardReference,
                        StandardName = certificate.StandardName,
                        Version = certificate.Version,
                        Level = certificate.StandardLevel,
                        AchievementDate = certificate.AchievementDate,
                        LearningStartDate = certificate.LearningStartDate,
                        ContactAddLine1 = certificate.ContactAddLine1,
                        ContactAddLine2 = certificate.ContactAddLine2,
                        ContactAddLine3 = certificate.ContactAddLine3,
                        ContactAddLine4 = certificate.ContactAddLine4,
                        ContactPostCode = certificate.ContactPostCode,
                        Status = certificate.Status,
                        ReasonForChange = printReasonForChange,
                        LatestStatusDatetime = latestCertificateStatusDate
                    };
                });

            var responses = certificateResponses?.ToList();
            if (responses != null)
            {
                foreach (var response in responses)
                {
                    var displayName = (await _contactQueryRepository.GetContact(response.RecordedBy))?.DisplayName;
                    response.RecordedBy = displayName ?? response.RecordedBy;
                }
            }

            var paginatedList = new PaginatedList<CertificateSummaryResponse>(responses,
                    certificates?.TotalRecordCount ?? 0,
                    certificates?.PageIndex ?? 1,
                    certificates?.PageSize ?? 10
                );

            return paginatedList;
        }
    }
}