namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class CreateOrganisationRequest : IRequest<OrganisationResponse>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
    }
}
