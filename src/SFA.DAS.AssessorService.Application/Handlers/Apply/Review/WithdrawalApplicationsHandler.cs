using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class WithdrawalApplicationsHandler : IRequestHandler<WithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<WithdrawalApplicationsHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public WithdrawalApplicationsHandler(IApplyRepository repository, ILogger<WithdrawalApplicationsHandler> logger, IStandardRepository standardRepository)
        {
            _repository = repository;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(WithdrawalApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving withdrawal applications");

            var organisationApplicationsResult = await _repository.GetWithdrawalApplications(request.OrganisationId, request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            var items = Mapper.Map<IEnumerable<ApplicationListItem>, IEnumerable<ApplicationSummaryItem>>(organisationApplicationsResult.PageOfResults);

            if (items.Any())
            {
                var allEnrolledVersions = await _standardRepository.GetEpaoRegisteredStandardVersions(items.First().EndPointAssessorOrganisationId);

                foreach (var item in items)
                {
                    var enrolledVersionsForStandard = allEnrolledVersions.Where(v => v.IFateReferenceNumber == item.StandardReference).ToList();
                    item.WithdrawalType = GetWithdrawalApplicationType(item, enrolledVersionsForStandard);
                }
            }


            return new PaginatedList<ApplicationSummaryItem>(items.ToList(),
                    organisationApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }

        // SV-912 Helper to generate the type of withdrawal
        private string GetWithdrawalApplicationType(ApplicationSummaryItem item, IEnumerable<OrganisationStandardVersion> enrolledVersionsForStandard)
        {
            if (item.StandardReference == null)
            {
                return WithdrawalTypes.Register;
            }
            else if (item.StandardApplicationType == StandardApplicationTypes.VersionWithdrawal)
            {
                return WithdrawalTypes.Version;
            }
            else if (item.StandardApplicationType == StandardApplicationTypes.StandardWithdrawal)
            {
                return WithdrawalTypes.Standard;
            }

            return null;
        }
    }
}
