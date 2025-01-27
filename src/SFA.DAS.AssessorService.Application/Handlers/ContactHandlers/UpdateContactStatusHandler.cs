using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class UpdateContactStatusHandler : IRequestHandler<UpdateContactStatusRequest, Unit>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateContactStatusHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Unit> Handle(UpdateContactStatusRequest updateContactStatusRequest, CancellationToken cancellationToken)
        {
            await _contactRepository.UpdateStatus(updateContactStatusRequest);

            return Unit.Value;
        }
    }
}
