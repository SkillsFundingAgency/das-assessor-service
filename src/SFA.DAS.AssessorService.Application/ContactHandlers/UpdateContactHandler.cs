﻿namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Interfaces;
    using MediatR;

    public class UpdateContactHandler : IRequestHandler<UpdateContactRequest>
    {
        private readonly IContactRepository _contactRepository;

        public UpdateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task Handle(UpdateContactRequest organisationUpdateViewModel, CancellationToken cancellationToken)
        {           
            await _contactRepository.Update(organisationUpdateViewModel);            
        }      
    }
}