using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateSignInId
{
    public class UpdateSignInIdHandler : IRequestHandler<UpdateSignInIdRequest>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IContactApplyClient _contactApplyClient;

        public UpdateSignInIdHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository, IContactApplyClient contactApplyClient)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _contactApplyClient = contactApplyClient;
        }

        public async Task Handle(UpdateSignInIdRequest request, CancellationToken cancellationToken)
        {
            var existingContact = await _contactQueryRepository.GetContactById(request.ContactId);

            await UpdateContactStatusToLive(existingContact);
            await UpdateContactRecordInApply(request);
            
            await _contactRepository.UpdateSignInId(existingContact.Id, request.SignInId);
        }

        private async Task UpdateContactRecordInApply(UpdateSignInIdRequest request)
        {
            await _contactApplyClient.SignInIdCallback(new SignInCallback() {SourceId = request.ContactId.ToString(), Sub = request.SignInId.ToString()});
        }

        private async Task UpdateContactStatusToLive(Contact existingContact)
        {
            await _contactRepository.UpdateStatus(existingContact.Id, ContactStatus.Live);
        }
    }
}
