﻿using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateEmail
{
    /// <summary>
    /// Handler responsible for updating the Email address of the Contact.
    /// </summary>
    public class UpdateEmailHandler : IRequestHandler<UpdateEmailRequest, Unit>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateEmailHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Unit> Handle(UpdateEmailRequest updateEmailRequest, CancellationToken cancellationToken)
        {
            await _contactRepository.UpdateEmail(updateEmailRequest);

            return Unit.Value;
        }
    }
}
