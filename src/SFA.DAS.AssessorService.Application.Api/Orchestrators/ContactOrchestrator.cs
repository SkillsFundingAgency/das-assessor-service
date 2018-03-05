using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    public class ContactOrchestrator
    {
        private readonly IStringLocalizer<ContactOrchestrator> _localizer;
        private readonly IMediator _mediator;

        public ContactOrchestrator(IMediator mediator,
            IStringLocalizer<ContactOrchestrator> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public async Task<Contact> CreateContact(
            [FromBody] CreateContactRequest createContactRequest)
        {
            var contact = await _mediator.Send(createContactRequest);
            return contact;
        }


        public async Task UpdateContact(
            [FromBody] UpdateContactRequest updateContactRequest)
        {
            await _mediator.Send(updateContactRequest);
        }

        public async Task DeleteContact(string userName)
        {
            try
            {
                var deleteContactRequest = new DeleteContactRequest()
                {
                    UserName = userName
                };

                await _mediator.Send(deleteContactRequest);
            }
            catch (NotFound)
            {
                throw new ResourceNotFoundException();
            }
        }
    }
}