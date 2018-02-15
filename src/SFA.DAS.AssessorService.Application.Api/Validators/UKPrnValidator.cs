namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;

    public class UkPrnValidator : AbstractValidator<int>
    {
        public UkPrnValidator()
        {
            RuleFor(ukPrn => ukPrn).InclusiveBetween(10000000, 99999999);
        }
    }
}
