﻿using System.Threading;
using System.Threading.Tasks;

using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class DeleteContactHandler : IRequestHandler<DeleteContactRequest, Unit>
    {
        private readonly IContactRepository _contactRepository;

        public DeleteContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Unit> Handle(DeleteContactRequest deleteContactRequest, CancellationToken cancellationToken)
        {
            await _contactRepository.Delete(deleteContactRequest.UserName);

            return Unit.Value;
        }
    }
}