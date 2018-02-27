namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Interfaces;

    public class DeleteContactHandler : IRequestHandler<DeleteContactRequest>
    {
        private readonly IContactRepository _contactRepository;

        public DeleteContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(DeleteContactRequest contactDeleteViewModel, CancellationToken cancellationToken)
        {
            await _contactRepository.Delete(contactDeleteViewModel.Id);
        }
    }
}