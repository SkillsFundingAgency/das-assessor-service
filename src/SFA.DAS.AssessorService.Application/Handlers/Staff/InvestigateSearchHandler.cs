using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class InvestigateSearchHandler: IRequestHandler<InvestigateSearchRequest, InvestigationResult>
    {
        private readonly IIlrRepository _ilrRepository;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IMediator _mediator;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public InvestigateSearchHandler(IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient, 
            IMediator mediator, IContactQueryRepository contactQueryRepository, IOrganisationQueryRepository organisationQueryRepository)
        {
            _ilrRepository = ilrRepository;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _mediator = mediator;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;
        }
        public async Task<InvestigationResult> Handle(InvestigateSearchRequest request, CancellationToken cancellationToken)
        {
            // First do a normal search.  If that comes back a-ok then return and say all is good.
            var contacts = await _contactQueryRepository.GetContacts(request.Epaorgid);
            var contact = contacts.First();
            var org = await _organisationQueryRepository.Get(contact.OrganisationId.Value);
            var searchResult = await _mediator.Send(new SearchQuery
            {
                Surname = request.Surname,
                Uln = request.Uln,
                Username = request.Username,
                UkPrn = org.EndPointAssessorUkprn.Value
            });
            if (searchResult.Any())
            {
                return new InvestigationResult() {Explanation = "", Ilr = searchResult};
            }
           
            // First search just by uln.
            
            // If it doesn't, then.....
            var resultByIlr = await _ilrRepository.SearchForLearnerByUln(request.Uln);
            if (!resultByIlr.Any())
            {
                // If no result, return "ULN does not exist in ILR data".
                return new InvestigationResult() {Explanation = "ULN does not exist in the ILR"};
            }
            else
            {
                // If result, add result to return.  If surname == GivenNames say "Surname / Firstname switched in ILR"
                if (resultByIlr.First().FamilyName != request.Surname)
                {
                    return new InvestigationResult()
                    {
                        Explanation =
                            $"ULN exists, but Surname of {request.Surname} does not match ILR FamilyName of {resultByIlr.First().FamilyName}"
                    };
                }
                else
                {
                    // If surname is good, get epao's standards and check standard code against ilr record.
                    var standards =
                        (await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(
                            org.EndPointAssessorOrganisationId)).Select(s => s.StandardCode).ToList();

                    var intStandards = new List<int>();
                    standards.ForEach(s =>
                    {
                        if (int.TryParse(s, out var intStandard))
                        {
                            intStandards.Add(intStandard);
                        }
                    });

                    if (!intStandards.Contains(resultByIlr.First().StdCode))
                    {
                        return new InvestigationResult()
                        {
                            Explanation =
                                $"ULN exists, but EPAO does not offer Standard Code: {resultByIlr.First().StdCode} as found on the learner record."
                        };
                    }
                }
            }


            return null;

        }
    }
}