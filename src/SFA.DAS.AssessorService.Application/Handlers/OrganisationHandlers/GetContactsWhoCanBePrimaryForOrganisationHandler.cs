using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetContactsWhoCanBePrimaryForOrganisationHandler : BaseHandler, IRequestHandler<GetContactsWhoCanBePrimaryForOrganisationRequest, List<ContactResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        public GetContactsWhoCanBePrimaryForOrganisationHandler(IContactQueryRepository contactQueryRepository, IMapper mapper) : base (mapper)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactResponse>> Handle(GetContactsWhoCanBePrimaryForOrganisationRequest request, CancellationToken cancellationToken)
        {
            var response = new List<ContactResponse>();
            var contacts = await _contactQueryRepository.GetContactsForEpao(request.EndPointAssessorOrganisationId);
            if (contacts == null)
                return response;
            return _mapper.Map<List<ContactResponse>>(
                contacts.Where(x => x.Status == ContactStatus.Live));
        }
    }
}


