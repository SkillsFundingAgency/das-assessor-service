using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UpdateStandardDataRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string StandardName { get; set; }
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
        public List<string> Versions { get; set; }
    }
}
