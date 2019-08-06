using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationPhoneNumberRequest : IRequest<bool>
    {
        public string PhoneNumber { get; set; }
        public string OrganisationId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
