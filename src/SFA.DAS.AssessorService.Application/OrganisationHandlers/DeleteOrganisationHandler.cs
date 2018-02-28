namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Interfaces;
    using MediatR;

    public class DeleteOrganisationHandler : IRequestHandler<DeleteOrganisationRequest>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public DeleteOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task Handle(DeleteOrganisationRequest organisationDeleteViewModel, CancellationToken cancellationToken)
        {
            await _organisationRepository.Delete(organisationDeleteViewModel.EndPointAssessorOrganisationId);
        }
    }
}