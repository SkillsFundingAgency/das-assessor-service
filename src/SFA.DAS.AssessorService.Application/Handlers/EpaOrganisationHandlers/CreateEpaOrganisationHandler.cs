using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

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
            return await _registerRepository.CreateEpaOrganisation(new EpaOrganisation());
        }
    }
}
