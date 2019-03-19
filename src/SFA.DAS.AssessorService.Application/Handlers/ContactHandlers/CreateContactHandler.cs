﻿using System;
using System.Linq;
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
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IDfeSignInService _dfeSignInService;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public CreateContactHandler(
            IOrganisationRepository organisationRepository,
            IOrganisationQueryRepository organisationQueryRepository,
            IContactRepository contactRepository,
            IContactQueryRepository contactQueryRepository,
            IDfeSignInService dfeSignInService,
            IMediator mediator)
        {
            _organisationRepository = organisationRepository;
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _dfeSignInService = dfeSignInService;
            _mediator = mediator;
        }

        public async Task<ContactBoolResponse> Handle(CreateContactRequest createContactRequest, CancellationToken cancellationToken)
        {
            Contact contactResponse = null;
            var response = new ContactBoolResponse(true);
            var newContact = Mapper.Map<Contact>(createContactRequest);           
            newContact.OrganisationId = null;
            newContact.Status = ContactStatus.New;

            var existingContact = await _contactRepository.GetContact(newContact.Email);
            if (existingContact == null)
            {
                contactResponse = await _contactRepository.CreateNewContact(newContact);
                //Todo: The role should be associated after the user has been created by another mechanism
                await _contactRepository.AssociateRoleWithContact("SuperUser", newContact);
                var privileges = await _contactQueryRepository.GetAllPrivileges();
                await _contactRepository.AssociatePrivilegesWithContact(contactResponse.Id, privileges);

                var invitationResult = await _dfeSignInService.InviteUser(createContactRequest.Email, createContactRequest.GivenName, createContactRequest.FamilyName, newContact.Id);
                if (!invitationResult.IsSuccess)
                {
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
                        response.Result = false;
                        return response;
                    }
                }
                // otherwise advise they already have an account (by Email)
               var emailTemplate =  await _mediator.Send(new GetEMailTemplateRequest {TemplateName = "ApplySignupError"}, cancellationToken);
               await _mediator.Send(new SendEmailRequest(createContactRequest.Email, emailTemplate, new { }), cancellationToken);
            }

            if (newContact
                .EndPointAssessorOrganisationId != null && !(await _organisationQueryRepository.CheckIfOrganisationHasContacts(newContact
                .EndPointAssessorOrganisationId)))
            {
                await SetOrganisationStatusToLiveAndSetPrimaryContact(createContactRequest, contactResponse);
            }

            return response;
        }

        private async Task SetOrganisationStatusToLiveAndSetPrimaryContact(CreateContactRequest createContactRequest, Contact contact)
        {
            var organisation =
                await _organisationQueryRepository.Get(createContactRequest.EndPointAssessorOrganisationId);
            
            organisation.PrimaryContact = contact.Username;
            organisation.Status = OrganisationStatus.Live;

            await _organisationRepository.UpdateOrganisation(organisation);
        }
    }
}