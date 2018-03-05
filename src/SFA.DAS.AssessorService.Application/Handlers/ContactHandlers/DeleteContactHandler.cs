using System.Threading;
using System.Threading.Tasks;

using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class DeleteContactHandler : IRequestHandler<DeleteContactRequest>
    {
        private readonly IContactRepository _contactRepository;

        public DeleteContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(DeleteContactRequest deleteContactRequest, CancellationToken cancellationToken)
        {
            await _contactRepository.Delete(deleteContactRequest.UserName);
        }
    }
}