﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, ContactBoolResponse>
    {
        
        private readonly IContactRepository _contactRepository;
        private readonly ISignInService _signInService;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateContactHandler> _logger;

        public CreateContactHandler(
            IContactRepository contactRepository,
            IContactQueryRepository contactQueryRepository,
            ISignInService signInService,
            IMediator mediator, ILogger<CreateContactHandler> logger)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _signInService = signInService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ContactBoolResponse> Handle(CreateContactRequest createContactRequest, CancellationToken cancellationToken)
        {
            var response = new ContactBoolResponse(true);
            var newContact = Mapper.Map<Contact>(createContactRequest);           
            newContact.OrganisationId = null;
            newContact.Status = ContactStatus.New;
            newContact.SignInType = string.IsNullOrEmpty(createContactRequest.GovIdentifier) ? "ASLogin" : "GovLogin";
            newContact.Title = "";
            newContact.GivenNames = createContactRequest.GivenName;
            newContact.GovUkIdentifier = createContactRequest.GovIdentifier;
            
            var existingContact = await _contactRepository.GetContact(newContact.Email);
            if (existingContact == null)
            {
                Contact contactResponse;
                try
                {
                    contactResponse = await _contactRepository.CreateNewContact(newContact);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"CreateContactHandler Error: {e.Message} {e.StackTrace} {e.InnerException?.Message}");
                    throw;
                }
                
                if (!string.IsNullOrEmpty(createContactRequest.GovIdentifier))
                {
                    await _contactRepository.UpdateSignInId(contactResponse.Id, Guid.NewGuid(), createContactRequest.GovIdentifier);
                    response.Result = true;
                    return response;
                }

                var invitationResult = await _signInService.InviteUser(createContactRequest.Email, createContactRequest.GivenName, createContactRequest.FamilyName, contactResponse.Id);
                if (!invitationResult.IsSuccess)
                {
                    if (invitationResult.UserExists)
                    {
                        await _contactRepository.UpdateSignInId(contactResponse.Id, invitationResult.ExistingUserId, null);
                        response.Result = true;
                        return response;
                    }
                    response.Result = false;
                    return response;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(createContactRequest.GovIdentifier))
                {
                    await _contactRepository.UpdateSignInId(existingContact.Id, Guid.NewGuid(), createContactRequest.GovIdentifier);
                    response.Result = true;
                    return response;
                }
                
                var invitationResult = await _signInService.InviteUser(createContactRequest.Email, createContactRequest.GivenName, createContactRequest.FamilyName, existingContact.Id);
                if (!invitationResult.IsSuccess)
                {
                    if (invitationResult.UserExists)
                    {
                        await _contactRepository.UpdateSignInId(existingContact.Id, invitationResult.ExistingUserId, null);
                        response.Result = true;
                        return response;
                    }
                    response.Result = false;
                    return response;
                }
            }
            return response;
        }
    }
}