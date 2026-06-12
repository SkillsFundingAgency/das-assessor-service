using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, ContactBoolResponse>
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<CreateContactHandler> _logger;

        public CreateContactHandler(IContactRepository contactRepository, ILogger<CreateContactHandler> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<ContactBoolResponse> Handle(CreateContactRequest request, CancellationToken cancellationToken)
        {
            var response = new ContactBoolResponse(true);

            try
            {
                var newContact = new Contact
                {
                    EndPointAssessorOrganisationId = request.EndPointAssessorOrganisationId,
                    Username = request.Username,
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    Title = string.Empty,
                    FamilyName = request.FamilyName,
                    GivenNames = request.GivenName,
                    GovUkIdentifier = request.GovIdentifier,
                    OrganisationId = null,
                    Status = ContactStatus.New,
                    SignInType = "GovLogin"
                };

                var existingContact = await _contactRepository.GetContact(newContact.Email);
                if (existingContact == null)
                {
                    Contact contactResponse = await _contactRepository.CreateNewContact(newContact);
                    if (!string.IsNullOrEmpty(request.GovIdentifier))
                    {
                        await _contactRepository.UpdateGovUkIdentifier(contactResponse.Id,  request.GovIdentifier);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.GovIdentifier))
                    {
                        await _contactRepository.UpdateGovUkIdentifier(existingContact.Id, request.GovIdentifier);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unable to create new contact for Email:{request.Email}, GovIdentifier:{request.GovIdentifier}");
                throw;
            }

            return response;
        }
    }
}