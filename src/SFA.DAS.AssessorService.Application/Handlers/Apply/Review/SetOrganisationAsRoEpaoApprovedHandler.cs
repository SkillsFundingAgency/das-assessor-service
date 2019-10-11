using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class SetOrganisationAsRoEpaoApprovedHandler : IRequestHandler<SetOrganisationAsRoEpaoApprovedRequest>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IOrganisationRepository _organisationRepository;

        public SetOrganisationAsRoEpaoApprovedHandler(IApplyRepository repository, IOrganisationQueryRepository organisationQueryRepository, IOrganisationRepository organisationRepository)
        {
            _applyRepository = repository;
            _organisationQueryRepository = organisationQueryRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<Unit> Handle(SetOrganisationAsRoEpaoApprovedRequest request, CancellationToken cancellationToken)
        {
            var application = await _applyRepository.GetApplication(request.ApplicationId);
            
            if (application != null && application.ApplicationStatus == ApplicationStatus.Approved)
            {
                var organisation = await _organisationQueryRepository.Get(request.OrganisationId);

                if (organisation?.OrganisationData != null && organisation.Id == application.Id)
                {
                    organisation.OrganisationData.RoEPAOApproved = true;

                    await _organisationRepository.UpdateOrganisation(organisation);
                }
            }

            return Unit.Value;
        }
    }
}
