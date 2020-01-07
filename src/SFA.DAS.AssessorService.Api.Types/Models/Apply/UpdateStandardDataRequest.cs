using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UpdateStandardDataRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string StandardName { get; set; }
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
    }
}
