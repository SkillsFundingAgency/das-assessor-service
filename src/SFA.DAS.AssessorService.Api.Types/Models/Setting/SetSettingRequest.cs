using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SetSettingRequest : IRequest<SetSettingResult>
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum SetSettingResult
    {
        Created,
        Updated
    }
}
