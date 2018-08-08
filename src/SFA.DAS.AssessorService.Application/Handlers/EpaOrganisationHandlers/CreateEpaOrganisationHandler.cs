using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
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
            if (string.IsNullOrEmpty(request.OrganisationId))
            {
                throw new BadRequestException("There is no organisation Id");
            }

            if (request.OrganisationId.Trim().Length>12)
            {
                throw new BadRequestException("The length of the organisation Id is too long");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BadRequestException("There is no organisation Name");
            }

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
