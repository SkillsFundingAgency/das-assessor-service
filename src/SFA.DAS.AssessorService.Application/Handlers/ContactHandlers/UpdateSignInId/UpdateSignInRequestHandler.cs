using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateSignInId
{
    public class UpdateSignInRequestHandler : IRequestHandler<UpdateSignInIdRequest>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;

        public UpdateSignInRequestHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task Handle(UpdateSignInIdRequest request, CancellationToken cancellationToken)
        {
            var existingContact = await _contactQueryRepository.GetContactById(request.ContactId);

            //Todo: The role should be associated after the user has been created by another mechanism
            await _contactRepository.AssociateRoleWithContact("SuperUser", existingContact);
            if (!_contactRepository.CheckIfAnyPrivelegesSet(existingContact.Id))
            {
                var privileges = await _contactQueryRepository.GetAllPrivileges();
                await _contactRepository.AssociatePrivilegesWithContact(existingContact.Id, privileges);
            }

            await _contactRepository.UpdateSignInId(existingContact.Id, request.SignInId);
            
        }
        
    }
}
