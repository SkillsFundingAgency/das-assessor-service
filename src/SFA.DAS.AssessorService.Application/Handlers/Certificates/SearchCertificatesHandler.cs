using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class SearchCertificatesHandler : IRequestHandler<SearchCertificatesRequest, List<SearchCertificatesResponse>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchCertificatesHandler> _logger;

        public SearchCertificatesHandler(ICertificateRepository certificateRepository, ILogger<SearchCertificatesHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<List<SearchCertificatesResponse>> Handle(SearchCertificatesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Certificate search requested: excludeCount={ExcludeCount}", request.Exclude?.Count() ?? 0);

            var results = await _certificateRepository.SearchByDobAndFamilyName(request.DateOfBirth, request.FamilyName, request.Exclude ?? Enumerable.Empty<long>());

            return results;
        }
    }
}
