using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class CreateApplicationRequest: IRequest<Guid>
    {
        public Guid QnaApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
    }
}
