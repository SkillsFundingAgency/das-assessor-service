﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class UpdateContactWithOrgAndStatus : IRequestHandler<UpdateContactWithOrgAndStausRequest, Unit>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public UpdateContactWithOrgAndStatus(IContactRepository contactRepository, IOrganisationRepository organisationRepository,
        IOrganisationQueryRepository organisationQueryRepository)
        {
            _contactRepository = contactRepository;
            _organisationRepository = organisationRepository;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<Unit> Handle(UpdateContactWithOrgAndStausRequest updateContactStatusRequest, CancellationToken cancellationToken)
        {
           var contact =  await _contactRepository.UpdateContactWithOrganisationData(updateContactStatusRequest);
            if (contact
                    .EndPointAssessorOrganisationId != null && !(await _organisationQueryRepository.CheckIfOrganisationHasContacts(contact
                    .EndPointAssessorOrganisationId)))
            {
                await SetOrganisationStatusToApplyAndSetPrimaryContact(contact
                    .EndPointAssessorOrganisationId, contact);
            }
            return Unit.Value;
        }

        private async Task SetOrganisationStatusToApplyAndSetPrimaryContact(string endPointAssessorOrganisationId, Contact contact)
        {
            var organisation =
                await _organisationQueryRepository.Get(endPointAssessorOrganisationId);

            organisation.PrimaryContact = contact.Username;
            organisation.Status = OrganisationStatus.Applying;

            await _organisationRepository.UpdateOrganisation(organisation);
        }
    }
}
