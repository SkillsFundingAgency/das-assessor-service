using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class OrgansationSearchHandler: IRequestHandler<OrganisationSearchRequest, IEnumerable<OrganisationSearchResult>>
    {
        private readonly ILogger<OrgansationSearchHandler> _logger;
        private readonly IWebConfiguration _config;
        public OrgansationSearchHandler(ILogger<OrgansationSearchHandler> logger,IWebConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<IEnumerable<OrganisationSearchResult>> Handle(OrganisationSearchRequest request, CancellationToken cancellationToken)
        {
            string searchTerm = request.SearchTerm;
            _logger.LogInformation("Handling Organisation Search Request");
            using (var httpClient = new HttpClient() { BaseAddress = new Uri(_config.ApplyApiAuthentication.ApiBaseAddress) })
            {

              return await (await httpClient.GetAsync($"/OrganisationSearch?searchTerm={searchTerm}")).Content.ReadAsAsync<IEnumerable<OrganisationSearchResult>>();
            }
        }
    }
}
