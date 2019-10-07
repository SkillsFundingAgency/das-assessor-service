using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Financial.Review
{
    public class ReturnFinancialReviewHandler : IRequestHandler<ReturnFinancialReviewRequest>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IOrganisationRepository _organisationRepository;

        public ReturnFinancialReviewHandler(IApplyRepository applyRepository, IOrganisationRepository organisationRepository)
        {
            _applyRepository = applyRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<Unit> Handle(ReturnFinancialReviewRequest request, CancellationToken cancellationToken)
        {           
            await _applyRepository.ReturnFinancialReview(request.Id, request.UpdatedGrade);
            await UpdateOrganisationWithUpdatedGrade(request.Id, request.UpdatedGrade);

            return Unit.Value;
        }

        private async Task UpdateOrganisationWithUpdatedGrade(Guid applicationId, FinancialGrade grade)
        {
            var application = await _applyRepository.GetApplication(applicationId);
            var org = application?.Organisation;

            if (org != null)
            {
                org.OrganisationData.FHADetails = new FHADetails()
                {
                    FinancialDueDate = grade.FinancialDueDate,
                    FinancialExempt = grade.SelectedGrade == FinancialApplicationSelectedGrade.Exempt
                };

                if (org.OrganisationData.FinancialGrades == null)
                    org.OrganisationData.FinancialGrades = new List<FinancialGrade>();

                if (org.OrganisationData.FinancialGrades.Any(x => x.ApplicationReference == grade.ApplicationReference))
                {
                    org.OrganisationData.FinancialGrades = org.OrganisationData.FinancialGrades.
                        Where(x => x.ApplicationReference == grade.ApplicationReference).Select(s => { return grade; }
                    ).ToList();
                }
                else
                {
                    org.OrganisationData.FinancialGrades.Add(grade);
                }

                await _organisationRepository.UpdateOrganisation(org);
            }
        }

    }
}
