using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationValidationRequest : IRequest<ValidationResponse>
    {
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string OrganisationId { get; set; }
    }
}
