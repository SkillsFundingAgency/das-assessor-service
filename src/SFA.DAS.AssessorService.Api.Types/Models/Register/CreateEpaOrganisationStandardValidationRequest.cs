using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class CreateEpaOrganisationStandardValidationRequest : IRequest<ValidationResponse>
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public List<string> StandardVersions { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ContactId { get; set; }
        public List<int> DeliveryAreas { get; set; }
        public string StandardApplicationType { get; set; }
    }
}
