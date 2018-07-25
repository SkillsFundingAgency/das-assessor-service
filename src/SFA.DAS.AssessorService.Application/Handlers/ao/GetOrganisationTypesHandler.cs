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
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<GetOrganisationTypesHandler> _logger;

        public GetOrganisationTypesHandler(IRegisterRepository registerRepository, ILogger<GetOrganisationTypesHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
        }


        public async Task<List<EpaOrganisationType>> Handle(GetOrganisationTypesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetOrganisationsType Request");
            var res = await _registerRepository.GetOrganisationTypes();
            return res.ToList();
        }
    }
}
