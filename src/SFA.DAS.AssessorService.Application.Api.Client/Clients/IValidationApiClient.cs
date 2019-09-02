using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IValidationApiClient
    {
        Task<bool> ValidatePhoneNumber(string phoneNumberToValidate);
        Task<bool> ValidateEmailAddress(string emailToValidate);
        Task<bool> ValidateWebsiteLink(string websiteLinkToValidate);
        
    }
}