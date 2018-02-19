namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class DeleteOrganisationHandler : IRequestHandler<OrganisationDeleteViewModel>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public DeleteOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task Handle(OrganisationDeleteViewModel organisationDeleteViewModel, CancellationToken cancellationToken)
        {
            await _organisationRepository.Delete(organisationDeleteViewModel.UKPrn);
        }
    }
}