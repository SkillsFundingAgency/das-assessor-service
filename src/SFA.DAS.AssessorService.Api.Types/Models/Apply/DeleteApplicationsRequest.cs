using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class DeleteApplicationsRequest: IRequest<bool>
    {
        public IEnumerable<Guid> ApplicationIds { get; set; }
        public Guid? DeletingContactId { get; set; }
    }
}
