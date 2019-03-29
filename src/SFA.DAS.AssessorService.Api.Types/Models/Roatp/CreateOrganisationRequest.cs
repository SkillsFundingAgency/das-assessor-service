namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    
    public class CreateOrganisationRequest : IRequest
    {
        public Organisation Organisation { get; set; }
        public string Username { get; set; }
    }
 }
