using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class UpdateContactHandler : IRequestHandler<UpdateContactRequest>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Unit> Handle(UpdateContactRequest updateContactRequest, CancellationToken cancellationToken)
        {
            await _contactRepository.Update(updateContactRequest);

            return Unit.Value;
        }
    }
}