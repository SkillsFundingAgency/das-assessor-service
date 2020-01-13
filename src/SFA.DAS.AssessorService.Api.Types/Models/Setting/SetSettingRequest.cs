using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SetSettingRequest : IRequest<SetSettingResult>
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class SetSettingResult
    {
        public SettingResult SettingResult { get; set; }
        public string ValidationMessage { get; set; }
    }

    public enum SettingResult
    {
        Created,
        Updated,
        Invalid
    }
}
