namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class DeleteOrganisationRequest : IRequest
    {
        public string EndPointAssessorOrganisationId { get; set; }                  
    }
}
