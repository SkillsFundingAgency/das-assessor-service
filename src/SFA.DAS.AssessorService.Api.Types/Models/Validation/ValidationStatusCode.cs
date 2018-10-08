using System.Runtime.Serialization;

namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public enum ValidationStatusCode
    {
        [EnumMember(Value = "BadRequest")]
        BadRequest,
        [EnumMember(Value = "AlreadyExists")]
        AlreadyExists
    }
}
