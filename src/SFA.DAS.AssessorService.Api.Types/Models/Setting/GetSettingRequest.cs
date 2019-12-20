using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetSettingRequest : IRequest<string>
    {
        public string Name { get; set; }
    }
}
