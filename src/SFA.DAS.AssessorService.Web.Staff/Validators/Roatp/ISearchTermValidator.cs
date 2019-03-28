namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using System.Threading.Tasks;

    public interface ISearchTermValidator
    {
        Task<ValidationResponse> ValidateSearchTerm(string searchTerm);
    }
}
