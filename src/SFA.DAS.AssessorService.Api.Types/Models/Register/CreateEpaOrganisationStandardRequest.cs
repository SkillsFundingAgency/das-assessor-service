using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class CreateEpaOrganisationStandardRequest : IRequest<string>
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public List<OrganisationStandardVersion> StandardVersions { get; set; }
        public DateTime DateStandardApprovedOnRegister { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Comments { get; set; }
        public string ContactId { get; set; }
        public List<int> DeliveryAreas { get; set; }
        public string DeliveryAreasComments { get; set; }
        public string StandardApplicationType { get; set; }
    }
}
