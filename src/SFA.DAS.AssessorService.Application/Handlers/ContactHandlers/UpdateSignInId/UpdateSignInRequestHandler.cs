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

        public UpdateSignInRequestHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(UpdateSignInIdRequest request, CancellationToken cancellationToken)
        {
            await _contactRepository.UpdateSignInId(request.ContactId, request.SignInId);
            
        }
        
    }
}
