
namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;

    public class DuplicateCompanyNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CompanyNumber { get; set; }
    }
}
