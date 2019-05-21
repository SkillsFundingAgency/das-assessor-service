using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi
{
    public class LearnerDetailForExternalApiHandler : IRequestHandler<LearnerDetailForExternalApiRequest, LearnerDetailForExternalApi>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LearnerDetailForExternalApiHandler> _logger;
        private readonly IIlrRepository _ilrRepository;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IContactQueryRepository _contactRepository;

        public LearnerDetailForExternalApiHandler(IMediator mediator, ILogger<LearnerDetailForExternalApiHandler> logger, 
                                    IIlrRepository ilrRepository, IOrganisationQueryRepository organisationRepository, IContactQueryRepository contactRepository)
        {
            _mediator = mediator;
            _ilrRepository = ilrRepository;
            _logger = logger;
            _organisationRepository = organisationRepository;
            _contactRepository = contactRepository;
        }

        private async Task<StandardCollation> GetCollatedStandard(string referenceNumber)
        {
            return await _mediator.Send(new GetCollatedStandardRequest { ReferenceNumber = referenceNumber });
        }

        private async Task<StandardCollation> GetCollatedStandard(int standardId)
        {
            return await _mediator.Send(new GetCollatedStandardRequest { StandardId = standardId });
        }

        public async Task<LearnerDetailForExternalApi> Handle(LearnerDetailForExternalApiRequest request, CancellationToken cancellationToken)
        {
            LearnerDetailForExternalApi learnerDetail = null;

            var standard = int.TryParse(request.Standard, out int standardCode) ? await GetCollatedStandard(standardCode) : await GetCollatedStandard(request.Standard);

            if (standard != null)
            {
                var learner = await _ilrRepository.Get(request.Uln, standard.StandardId.GetValueOrDefault());

                if (learner != null)
                {
                    var epao = await _organisationRepository.GetByUkPrn(learner.UkPrn);
                    var primaryContact = await _contactRepository.GetContact(epao.PrimaryContact);

                    if(primaryContact is null)
                    {
                        var contacts = await _contactRepository.GetAllContacts(epao.EndPointAssessorOrganisationId);
                        primaryContact = contacts.FirstOrDefault();
                    }

                    learnerDetail = new LearnerDetailForExternalApi()
                    {
                        Uln = learner.Uln,
                        StandardCode = standard.StandardId,
                        StandardReference = standard.ReferenceNumber,
                        EndPointAssessorOrganisationId = epao.EndPointAssessorOrganisationId,
                        PrimaryContactEmail = primaryContact?.Email,
                        UkPrn = epao.EndPointAssessorUkprn.GetValueOrDefault()
                    };
                }
            }

            return learnerDetail;
        }
    }
}
