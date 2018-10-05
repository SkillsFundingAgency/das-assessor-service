using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationContactHandler : IRequestHandler<CreateOrganisationContactRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationContactHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public CreateEpaOrganisationContactHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ISpecialCharacterCleanserService cleanser, ILogger<CreateEpaOrganisationContactHandler> logger)
        {
            _registerRepository = registerRepository;
            _validator = validator;
            _cleanser = cleanser;
            _logger = logger;
        }

        public Task<string> Handle(CreateOrganisationContactRequest request, CancellationToken cancellationToken)
        {

            throw new System.NotImplementedException();
        }
    }
}
