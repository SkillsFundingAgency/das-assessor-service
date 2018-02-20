namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class UpdateContactHandler : IRequestHandler<ContactUpdateViewModel>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(ContactUpdateViewModel organisationUpdateViewModel, CancellationToken cancellationToken)
        {           
            await _contactRepository.Update(organisationUpdateViewModel);            
        }      
    }
}