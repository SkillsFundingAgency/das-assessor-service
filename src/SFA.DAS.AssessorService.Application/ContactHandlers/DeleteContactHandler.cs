namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class DeleteContactHandler : IRequestHandler<ContactDeleteViewModel>
    {
        private readonly IContactRepository _contactRepository;

        public DeleteContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(ContactDeleteViewModel contactDeleteViewModel, CancellationToken cancellationToken)
        {
            await _contactRepository.Delete(contactDeleteViewModel.Id);
        }
    }
}