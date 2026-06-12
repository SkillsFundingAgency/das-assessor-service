using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards
{
    public class OrganisationStandardAddRequest : IRequest<string>
    {
        public string OrganisationId { get; set; }
        public string StandardReference { get; set; }
        public List<string> StandardVersions { get; set; }
        public Guid ContactId { get; set; }
    }
}
