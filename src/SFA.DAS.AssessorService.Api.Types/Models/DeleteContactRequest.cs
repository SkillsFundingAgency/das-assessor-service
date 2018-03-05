namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using MediatR;

    public class DeleteContactRequest : IRequest
    {
        public string UserName { get; set; }                  
    }
}
