using SFA.DAS.AssessorService.Application.Interfaces;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class EpaOrganisationIdGenerator : IEpaOrganisationIdGenerator
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public EpaOrganisationIdGenerator(IRegisterQueryRepository registerQueryRepository)
        {
            _registerQueryRepository = registerQueryRepository;
        }

        public string GetNextOrganisationId()
        {
            var currentMaxId = _registerQueryRepository.EpaOrganisationIdCurrentMaximum().Result;
            if (currentMaxId == null)
                return "EPA0200";

            return int.TryParse(currentMaxId.Replace("EPA", string.Empty), out int currentIntValue) 
                ? $@"EPA{currentIntValue + 1:D4}" : 
                string.Empty;
        }
    }
}
