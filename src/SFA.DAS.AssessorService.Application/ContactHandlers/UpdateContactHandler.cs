namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class UpdateContactHandler : IRequestHandler<UpdateContactRequest>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(UpdateContactRequest organisationUpdateViewModel, CancellationToken cancellationToken)
        {           
            await _contactRepository.Update(organisationUpdateViewModel);            
        }      
    }
}