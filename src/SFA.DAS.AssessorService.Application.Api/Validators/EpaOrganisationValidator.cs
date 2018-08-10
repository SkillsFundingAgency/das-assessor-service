using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterRepository _registerRepository;
        public EpaOrganisationValidator( IRegisterRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }

        public string CheckOrganisationId(string organisationId)
        {
            
            if (string.IsNullOrEmpty(organisationId) || organisationId.Trim().Length==0)
            {
               return "There is no organisation Id; ";
            }

            if (organisationId.Trim().Length > 12)
            {
                return  "The length of the organisation Id is too long; ";
            }

            return string.Empty;
        }

        public string CheckOrganisationName(string organisationName)
        {
            if (string.IsNullOrEmpty(organisationName) || organisationName.Trim().Length==0)
                return "There is no organisation name; ";

            return string.Empty;
        }

        public async Task<string> CheckIfOrganisationIdExists(string organisationId)
        {
            if (organisationId == null ||
                !await _registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId)) return string.Empty;
            return $@"There is already an entry for [{organisationId}]; ";
        }

        public async Task<string> CheckIfOrganisationUkprnExists(long? ukprn)
        {
            if (ukprn == null || !await _registerRepository.EpaOrganisationExistsWithUkprn(ukprn.Value)) return string.Empty;
            return $@"There is already an organisation with ukprn: [{ukprn.Value}]; ";
        }
    }
}
