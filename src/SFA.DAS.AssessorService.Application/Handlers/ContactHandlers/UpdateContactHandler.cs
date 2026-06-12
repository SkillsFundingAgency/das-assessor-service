using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Handlers.ContactHandlers
{
    public class UpdateContactHandler : IRequestHandler<UpdateContactRequest, Unit>
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