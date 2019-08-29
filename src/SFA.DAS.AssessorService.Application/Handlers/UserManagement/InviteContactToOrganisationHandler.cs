using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class InviteContactToOrganisationHandler : IRequestHandler<InviteContactToOrganisationRequest, InviteContactToOrganisationResponse>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IMediator _mediator;
        private readonly ISignInService _signInService;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public InviteContactToOrganisationHandler(IContactQueryRepository contactQueryRepository,
            IContactRepository contactRepository,
            IMediator mediator, ISignInService signInService, IOrganisationQueryRepository organisationQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
            _contactRepository = contactRepository;
            _mediator = mediator;
            _signInService = signInService;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<InviteContactToOrganisationResponse> Handle(InviteContactToOrganisationRequest request, CancellationToken cancellationToken)
        {
            var existingContact = await _contactQueryRepository.GetContactFromEmailAddress(request.Email);
            if (existingContact != null)
            {
                return new InviteContactToOrganisationResponse()
                {
                    Success = false,
                    ErrorMessage = existingContact.OrganisationId == request.OrganisationId
                        ? "This email address is already registered against your organisation. You must use a unique email address."
                        : "This email address is already registered against another organisation. You must use a unique email address."
                };
            }

            var organisation = await _organisationQueryRepository.Get(request.OrganisationId);
            
            var newContact = await CreateNewContact(request, organisation);

            var inviter = await _contactQueryRepository.GetContactById(request.InvitedByContactId);
                
            await _signInService.InviteUserToOrganisation(request.Email, request.GivenName, request.FamilyName, newContact.Id, organisation.EndPointAssessorName, inviter.DisplayName);

            await _contactRepository.AddContactInvitation(request.InvitedByContactId, newContact.Id, organisation.Id);
            
            return new InviteContactToOrganisationResponse {Success = true, ContactId = newContact.Id};
        }

        private async Task<Contact> CreateNewContact(InviteContactToOrganisationRequest request, Organisation organisation)
        {
            var newContact = new Contact()
            {
                OrganisationId = request.OrganisationId,
                Email = request.Email,
                GivenNames = request.GivenName,
                FamilyName = request.FamilyName,
                DisplayName = request.GivenName + " " + request.FamilyName,
                Status = ContactStatus.Pending,
                SignInType = "ASLogin",
                Title = "",
                Username = request.Email, 
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
            };

            var contact = await _contactRepository.CreateNewContact(newContact);
            
           
            await _mediator.Send(new SetContactPrivilegesRequest
            {
                AmendingContactId = request.InvitedByContactId,
                ContactId = contact.Id,
                PrivilegeIds = request.Privileges,
                IsNewContact = true
            }, CancellationToken.None);

            return contact;
        }
    }
}