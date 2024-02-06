using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class OptOutStandardVersionViewModelValidator : AbstractValidator<OptOutStandardVersionViewModel>
    {
        public const string OneApprovedVersionRequiredMessage = "One approved standard version is required";

        public OptOutStandardVersionViewModelValidator(IHttpContextAccessor httpContextAccessor, IStandardVersionApiClient standardVersionApiClient)
        {
            var epaoOrgId = EpaOrgIdFinder.GetFromClaim(httpContextAccessor);

            RuleFor(vm => vm.StandardReference)
                .NotEmpty()
                .Must((vm, _) =>
                {
                    return standardVersionApiClient.GetEpaoRegisteredStandardVersions(epaoOrgId, vm.StandardReference)
                                                   .GetAwaiter()
                                                   .GetResult()
                                                   .FirstOrDefault(p => p.Version.Equals(vm.Version, StringComparison.InvariantCultureIgnoreCase)) != null;
                })
                .WithMessage(vm => $"You cannot opt out of {vm.StandardTitle}, {vm.StandardReference} as you have already opted out.")
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
}
