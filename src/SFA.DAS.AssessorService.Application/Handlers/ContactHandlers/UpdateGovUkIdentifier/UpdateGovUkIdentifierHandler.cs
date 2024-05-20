using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateGovUkidentifier
{
    public class UpdateGovUkidentifierHandler : IRequestHandler<UpdateGovUkIdentifierRequest>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;

        public UpdateGovUkidentifierHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<Unit> Handle(UpdateGovUkIdentifierRequest request, CancellationToken cancellationToken)
        {
            var existingContact = await _contactQueryRepository.GetContactById(request.ContactId);

            await UpdateContactStatusToLive(existingContact);
            
            await _contactRepository.UpdateGovUkIdentifier(existingContact.Id, request.GovIdentifier);
            return Unit.Value;
        }
        private async Task UpdateContactStatusToLive(Contact existingContact)
        {
            await _contactRepository.UpdateStatus(existingContact.Id, ContactStatus.Live);
        }
    }
}
