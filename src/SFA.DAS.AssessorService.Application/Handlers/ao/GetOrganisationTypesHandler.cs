using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.ao;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetOrganisationTypesHandler : IRequestHandler<GetOrganisationTypesRequest, List<EpaOrganisationType>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetOrganisationTypesHandler> _logger;

        public GetOrganisationTypesHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetOrganisationTypesHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<EpaOrganisationType>> Handle(GetOrganisationTypesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetOrganisationsType Request");
            var res = await _registerQueryRepository.GetOrganisationTypes();
            return res.ToList();
        }
    }
}
