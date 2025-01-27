using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class UpdateOrganisationHandler : BaseHandler, IRequestHandler<UpdateOrganisationRequest, Organisation>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationHandler(IOrganisationRepository organisationRepository, IMapper mapper) :  base(mapper)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<Organisation> Handle(UpdateOrganisationRequest updateOrganisationRequest, CancellationToken cancellationToken)
        {
            var organisation = _mapper.Map<Organisation>(updateOrganisationRequest);
            return await _organisationRepository.UpdateOrganisation(organisation);
        }
    }
}