using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class CreateApplicationRequest: IRequest<Guid>
    {
        public string ApplicationType { get; set; }
        public Guid QnaApplicationId { get; set; }
        public List<ApplySequence> ApplySequences { get; set; }
        public Guid OrganisationId { get; set; }
        public string ApplicationReferenceFormat { get; set; }
        public Guid CreatingContactId { get; set; }
    }
}
