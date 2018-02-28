namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class DeleteOrgananisationRequest : IRequest
    {
        public string EndPointAssessorOrganisationId { get; set; }                  
    }
}
