using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;

public class UpdateContactGovLoginRequestHandler : IRequestHandler<UpdateContactGovLoginRequest,UpdateContactGovLoginResponse>
{
    private readonly IContactRepository _contactRepository;

    public UpdateContactGovLoginRequestHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }
    public async Task<UpdateContactGovLoginResponse> Handle(UpdateContactGovLoginRequest request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.UpdateSignInId(request.ContactId, request.SignInId, request.GovIdentifier);
        
        return new UpdateContactGovLoginResponse
        {
            Contact = contact
        };
    }
}