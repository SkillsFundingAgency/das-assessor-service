namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Interfaces;
    using MediatR;

    public class DeleteOrganisationHandler : IRequestHandler<DeleteOrgananisationRequest>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public DeleteOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task Handle(DeleteOrgananisationRequest organisationDeleteViewModel, CancellationToken cancellationToken)
        {
            await _organisationRepository.Delete(organisationDeleteViewModel.EndPointAssessorOrganisationId);
        }
    }
}