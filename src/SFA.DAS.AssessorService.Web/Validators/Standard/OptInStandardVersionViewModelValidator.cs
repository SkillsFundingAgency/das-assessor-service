using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class OptInStandardVersionViewModelValidator : AbstractValidator<OptInStandardVersionViewModel>
    {
        public OptInStandardVersionViewModelValidator(IHttpContextAccessor httpContextAccessor, IStandardVersionClient standardVersionApiClient)
        {
            var epaoOrgId = EpaOrgIdFinder.GetFromClaim(httpContextAccessor);

            RuleFor(vm => vm.StandardReference)
                .Must((vm, _) =>
                {
                    var result = standardVersionApiClient.GetEpaoRegisteredStandardVersions(epaoOrgId, vm.StandardReference)
                                                   .GetAwaiter()
                                                   .GetResult();
                   return result.FirstOrDefault(p => p.Version.Equals(vm.Version, StringComparison.InvariantCultureIgnoreCase)) == null;
                })
                .WithMessage(vm => $"You cannot opt in to {vm.StandardTitle}, {vm.StandardReference} as you have already opted in.");
        }
    }
}
