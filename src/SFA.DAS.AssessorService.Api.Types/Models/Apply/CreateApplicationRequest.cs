using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class CreateApplicationRequest: ApplicationApplyData, IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }
        public List<ApplySequence> listOfApplySequences { get; set; }
    }
}
