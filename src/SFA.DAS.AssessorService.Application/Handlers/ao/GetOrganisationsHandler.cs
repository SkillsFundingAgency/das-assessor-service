using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.ao;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
  
    public class GetOrganisationsHandler : IRequestHandler<GetOrganisationsRequest, List<EpaOrganisationType>>
    {
        private readonly IRegisterRepository _registerRepository;

        public GetOrganisationsHandler(IRegisterRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }


        public async Task<List<EpaOrganisationType>> Handle(GetOrganisationsRequest request, CancellationToken cancellationToken)
        {
            var res = await _registerRepository.GetOrganisationTypes();
            return res.ToList();
        }
    }
}
