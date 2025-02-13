using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;
using System.Linq;
using Domain = SFA.DAS.AssessorService.Domain.Entities;

public class DeliveryAreasDetailsResolver : IValueResolver<Domain.Organisation, OrganisationStandardResponse, List<OrganisationStandardDeliveryArea>>
{
    public List<OrganisationStandardDeliveryArea> Resolve(
        Domain.Organisation source, 
        OrganisationStandardResponse destination, 
        List<OrganisationStandardDeliveryArea> destMember, 
        ResolutionContext context)
    {
        if (source.OrganisationStandards != null && source.OrganisationStandards.Any())
        {
            var firstStandard = source.OrganisationStandards.First();

            var mappedDeliveryAreas = firstStandard.OrganisationStandardDeliveryAreas
                .Select(area => context.Mapper.Map<Domain.OrganisationStandardDeliveryArea, OrganisationStandardDeliveryArea>(area))
                .ToList();

            return mappedDeliveryAreas;
        }
        else
        {
            return new List<OrganisationStandardDeliveryArea>();
        }
    }
}
