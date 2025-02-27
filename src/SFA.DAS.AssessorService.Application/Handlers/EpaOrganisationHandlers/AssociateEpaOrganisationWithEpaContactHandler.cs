using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class AssociateEpaOrganisationWithEpaContactHandler : IRequestHandler<AssociateEpaOrganisationWithEpaContactRequest, bool>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly IRegisterRepository _registerRepository;

        public AssociateEpaOrganisationWithEpaContactHandler(IRegisterQueryRepository registerQueryRepository, IRegisterRepository registerRepository)
        {
            _registerQueryRepository = registerQueryRepository;
            _registerRepository = registerRepository;
        }

        public async Task<bool> Handle(AssociateEpaOrganisationWithEpaContactRequest request, CancellationToken cancellationToken)
        {
            var success = false;

            var contact = await _registerQueryRepository.GetContactByContactId(request.ContactId);
            var organisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(request.OrganisationId);

            if (contact != null && organisation != null)
            {
                await _registerRepository.AssociateOrganisationWithContact(contact.Id, organisation, request.ContactStatus, request.MakePrimaryContact ? "MakePrimaryContact" : string.Empty);

                if (request.AddDefaultPrivileges)
                {
                    await _registerRepository.AssociateDefaultPrivilegesWithContact(contact);
                }

                success = true;
            }

            return success;
        }
    }
}
