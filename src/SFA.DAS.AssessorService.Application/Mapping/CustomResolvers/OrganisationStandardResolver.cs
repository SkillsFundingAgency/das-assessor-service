using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
    public class OrganisationStandardResolver : IValueResolver<Domain.Entities.Organisation, OrganisationStandardResponse, OrganisationStandard>
    {
        public OrganisationStandard Resolve(Domain.Entities.Organisation source, OrganisationStandardResponse destination, OrganisationStandard destMember, ResolutionContext context)
        {
            var firstStandard = source.OrganisationStandards.FirstOrDefault();
            if (firstStandard != null)
            {
                var mappedObject =  context.Mapper.Map<AssessorService.Api.Types.Models.AO.OrganisationStandard>(firstStandard);
                return mappedObject;
            }
            return new OrganisationStandard();
        }
    }
}
