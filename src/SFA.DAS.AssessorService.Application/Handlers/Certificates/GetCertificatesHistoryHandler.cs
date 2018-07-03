using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.AssessorService.Paging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesHistoryHandler : IRequestHandler<GetCertificateHistoryRequest, PaginatedList<CertificateHistoryResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;

        public GetCertificatesHistoryHandler(ICertificateRepository certificateRepository,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _logger = logger;
        }

        public async Task<PaginatedList<CertificateHistoryResponse>> Handle(GetCertificateHistoryRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 10;

            var certificates = await _certificateRepository.GetCertificateHistory(
                request.PageIndex ?? 1,
                pageSize);

            // Please Note:- Cannot seem to automap this with custom value/type converters
            // so dealing with it manually for now.
            var certificateHistoryResponses = MapCertificates(certificates);

            return certificateHistoryResponses;
        }

        private PaginatedList<CertificateHistoryResponse> MapCertificates(PaginatedList<Certificate> certificates)
        {
            var trainingProviderName = string.Empty;
            var certificateResponses = certificates.Items.Select(
                q =>
                {
                    var certificateData = JsonConvert.DeserializeObject<CertificateData>(q.CertificateData);

                    try
                    {
                        if (q.Organisation.EndPointAssessorUkprn.HasValue)
                        {
                            //var provider = _assessmentOrgsApiClient
                            //    .GetProvider(q.Organisation.EndPointAssessorUkprn.Value).GetAwaiter()
                            //    .GetResult();
                            var provider = _assessmentOrgsApiClient
                                .GetProvider(10000268).GetAwaiter()
                                .GetResult();
                            trainingProviderName = provider.ProviderName;
                        }
                    }
                    catch (EntityNotFoundException e)
                    {
                        _logger.LogInformation(
                            $"Cannot find training provider for ukprn {q.Organisation.EndPointAssessorUkprn.Value}");
                    }

                    return new CertificateHistoryResponse
                    {
                        CertificateReference = q.CertificateReference,
                        Uln = q.Uln,
                        CreatedAt = q.CreatedAt,
                        ContactOrganisation = q.Organisation.EndPointAssessorName,
                        ContactName = certificateData.ContactName,
                        TrainingProvider = trainingProviderName,
                        CourseOption = certificateData.CourseOption,
                        FullName = certificateData.FullName,
                        OverallGrade = certificateData.OverallGrade,
                        StandardName = certificateData.StandardName,
                        AchievementDate = certificateData.AchievementDate,
                        ContactAddLine1 = certificateData.ContactAddLine1,
                        ContactAddLine2 = certificateData.ContactAddLine2,
                        ContactAddLine3 = certificateData.ContactAddLine3,
                        ContactAddLine4 = certificateData.ContactAddLine4,
                        ContactPostCode = certificateData.ContactPostCode
                    };
                });

            var responses = certificateResponses.ToList();
            var paginatedList = new PaginatedList<CertificateHistoryResponse>(responses,
                    certificates.TotalRecordCount,
                    certificates.PageIndex,
                    certificates.PageSize
                );

            return paginatedList;
        }
    }
}