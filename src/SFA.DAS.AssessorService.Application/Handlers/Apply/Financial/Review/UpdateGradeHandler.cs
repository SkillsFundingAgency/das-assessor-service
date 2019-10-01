using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Financial.Review
{
    public class UpdateGradeHandler : IRequestHandler<UpdateGradeRequest>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public UpdateGradeHandler(IApplyRepository applyRepository,
            IOrganisationRepository organisationRepository, IOrganisationQueryRepository organisationQueryRepository)
        {
            _applyRepository = applyRepository;
            _organisationRepository = organisationRepository;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<Unit> Handle(UpdateGradeRequest request, CancellationToken cancellationToken)
        {
           
            await _applyRepository.UpdateApplicationFinancialGrade(request.Id, request.UpdatedGrade);

            await _applyRepository.UpdateApplicationSectionStatus(request.Id, "0","2", ApplicationSectionStatus.Graded);

            var org = await _organisationQueryRepository.Get(request.OrgId);

            org.OrganisationData.FHADetails = new FHADetails()
            {
                FinancialDueDate = request.UpdatedGrade.FinancialDueDate,
                FinancialExempt = request.UpdatedGrade.SelectedGrade == FinancialApplicationSelectedGrade.Exempt
            };

            if (org.OrganisationData.FinancialGrades == null)
                org.OrganisationData.FinancialGrades = new List<FinancialGrade>();

            if (org.OrganisationData.FinancialGrades.Any( x => x.ApplicationReference == request.UpdatedGrade.ApplicationReference))
            {
                org.OrganisationData.FinancialGrades=org.OrganisationData.FinancialGrades.
                    Where(x => x.ApplicationReference == request.UpdatedGrade.ApplicationReference).Select( s => { return request.UpdatedGrade; }
                ).ToList();
            }
            else
                 org.OrganisationData.FinancialGrades.Add(request.UpdatedGrade);

            await _organisationRepository.UpdateOrganisation(org);

            return Unit.Value;
        }

    }
}
