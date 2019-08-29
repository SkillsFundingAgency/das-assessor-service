using FluentValidation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class WebsiteLinkCheckValidator : AbstractValidator<WebsiteLinkCheck>
    {
        public WebsiteLinkCheckValidator()
        {
            DefaultValidatorExtensions.Matches<WebsiteLinkCheck>(RuleFor(x => x.WebsiteLinkToCheck), @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        }
    }
}