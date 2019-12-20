using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SetSettingRequest : IRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
