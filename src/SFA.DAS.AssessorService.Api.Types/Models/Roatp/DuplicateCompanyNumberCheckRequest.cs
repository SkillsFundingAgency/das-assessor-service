using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class DuplicateCompanyNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CompanyNumber { get; set; }
    }
}
