using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UpdateInitialStandardDataRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string StandardName { get; set; }
        public int StandardCode { get; set; }
    }
}
