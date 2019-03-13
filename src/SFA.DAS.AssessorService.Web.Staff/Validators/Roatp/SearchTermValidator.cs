namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Blob.Protocol;
    using Resources;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;

    public class SearchTermValidator : ISearchTermValidator
    {
        public async Task<ValidationResponse> ValidateSearchTerm(string searchTerm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };


            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("SearchTerm", RoatpSearchValidation.SearchTermMandatory));
            }
            else
            {
                if (searchTerm.Trim().Length < 2)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("SearchTerm",
                        RoatpSearchValidation.SearchTermLength));
                }
            }

            return await Task.FromResult(validationResponse);
        }
    }
}
