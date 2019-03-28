using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, ContactBoolResponse>
    {
        
        private readonly IContactRepository _contactRepository;
        private readonly IDfeSignInService _dfeSignInService;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public CreateContactHandler(
            IContactRepository contactRepository,
            IContactQueryRepository contactQueryRepository,
            IDfeSignInService dfeSignInService,
            IMediator mediator)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _dfeSignInService = dfeSignInService;
            _mediator = mediator;
        }

        public async Task<ContactBoolResponse> Handle(CreateContactRequest createContactRequest, CancellationToken cancellationToken)
        {
            var response = new ContactBoolResponse(true);
            var newContact = Mapper.Map<Contact>(createContactRequest);           
            newContact.OrganisationId = null;
            newContact.Status = ContactStatus.New;

            var existingContact = await _contactRepository.GetContact(newContact.Email);
            if (existingContact == null)
            {
                var contactResponse = await _contactRepository.CreateNewContact(newContact);
                //Todo: The role should be associated after the user has been created by another mechanism
                await _contactRepository.AssociateRoleWithContact("SuperUser", newContact);
                var privileges = await _contactQueryRepository.GetAllPrivileges();
                await _contactRepository.AssociatePrivilegesWithContact(contactResponse.Id, privileges);

                var invitationResult = await _dfeSignInService.InviteUser(createContactRequest.Email, createContactRequest.GivenName, createContactRequest.FamilyName, contactResponse.Id);
                if (!invitationResult.IsSuccess)
                {
                    if (invitationResult.UserExists)
                    {
                        await _contactRepository.UpdateSignInId(contactResponse.Id, invitationResult.ExistingUserId);
                        response.Result = true;
                        return response;
                    }
                    response.Result = false;
                    return response;
                }
            }
            else
            {
                if (existingContact.SignInId == null)
                {
                    var invitationResult = await _dfeSignInService.InviteUser(createContactRequest.Email, createContactRequest.GivenName, createContactRequest.FamilyName, existingContact.Id);
                    if (!invitationResult.IsSuccess)
                    {
                        if (invitationResult.UserExists)
                        {
                            await _contactRepository.UpdateSignInId(existingContact.Id, invitationResult.ExistingUserId);
                            response.Result = true;
                            return response;
                        }
                        response.Result = false;
                        return response;
                    }
                }
                // otherwise advise they already have an account (by Email)
               var emailTemplate =  await _mediator.Send(new GetEMailTemplateRequest {TemplateName = "AssessorSignupError"}, cancellationToken);
               await _mediator.Send(new SendEmailRequest(createContactRequest.Email, emailTemplate, new { }), cancellationToken);
            }
            return response;
        }
    }
}