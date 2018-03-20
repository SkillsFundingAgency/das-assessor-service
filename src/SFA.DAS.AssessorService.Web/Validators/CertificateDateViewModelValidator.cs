using System;
using FluentValidation;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateDateViewModelValidator : AbstractValidator<CertificateDateViewModel>
    {
        private readonly ICertificateApiClient _certApiClient;

        public CertificateDateViewModelValidator(ICertificateApiClient certApiClient)
        {
            _certApiClient = certApiClient;
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (int.TryParse(vm.Day, out var day) && int.TryParse(vm.Month, out var month) && int.TryParse(vm.Year, out var year))
                {
                    try
                    {
                        var achievementDate = new DateTime(year, month, day);

                        if (achievementDate > SystemTime.UtcNow())
                        {
                            context.AddFailure("Date", "Achievement Date cannot be in the future.");
                        }
                        else
                        {
                            var certificate = _certApiClient.GetCertificate(vm.Id).Result;
                            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                            if (achievementDate < certData.LearningStartDate.AddMonths(12))
                            {
                                context.AddFailure("Date", $"Achievement Date must be at least 12 months from Learner Start Date of {certData.LearningStartDate}.");
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        context.AddFailure("Date", "Date is not in correct format.");
                    }
                }
                else
                {
                    context.AddFailure("Date", "Date is not in correct format.");
                }
            });
        }
    }
}