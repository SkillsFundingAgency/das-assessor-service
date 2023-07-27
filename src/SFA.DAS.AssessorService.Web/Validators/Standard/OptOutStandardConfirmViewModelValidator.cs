using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard;

public class OptOutStandardConfirmViewModelValidator : AbstractValidator<OptOutStandardVersionViewModel>
{
    public const string OneApprovedVersionRequiredMessage = "One approved standard version is required";

    public OptOutStandardConfirmViewModelValidator(IHttpContextAccessor httpContextAccessor, IStandardVersionClient standardVersionApiClient)
    {
        var epaoOrgId = EpaOrgIdFinder.GetFromClaim(httpContextAccessor);

        RuleFor(x => x.StandardReference)
            .Must((standardReference) =>
            {
                var approvedVersions = standardVersionApiClient.GetEpaoRegisteredStandardVersions(epaoOrgId, standardReference)
                                                               .GetAwaiter()
                                                               .GetResult();

                return approvedVersions.Count() > 1;
            })
           .WithMessage(OneApprovedVersionRequiredMessage);
    }
}
