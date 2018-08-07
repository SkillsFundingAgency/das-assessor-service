using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationHandler : IRequestHandler<CreateEpaOrganisationRequest, EpaOrganisation>
    {
        private readonly IRegisterRepository _registerRepository;

        public CreateEpaOrganisationHandler(IRegisterRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }

        public async Task<EpaOrganisation> Handle(CreateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            var organisation = Mapper.Map<EpaOrganisation>(request);
            organisation.Status = OrganisationStatus.New;
            organisation.Id = Guid.NewGuid();

            organisation.OrganisationData = new OrganisationData
            {
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                Address4 = request.Address4,
                LegalName = request.LegalName,
                Postcode = request.Postcode,
                WebsiteLink = request.WebsiteLink
            };

            return await _registerRepository.CreateEpaOrganisation(organisation);
        }
    }
}
