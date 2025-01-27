namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class DeleteOrganisationRequest : IRequest<Unit>
    {
        public string EndPointAssessorOrganisationId { get; set; }                  
    }
}
