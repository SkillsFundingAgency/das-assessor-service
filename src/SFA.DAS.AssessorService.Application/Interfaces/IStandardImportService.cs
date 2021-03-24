using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardImportService
    {
        Task LoadStandards(IEnumerable<GetStandardByIdResponse> standards);
        Task UpsertStandardCollations(IEnumerable<GetStandardByIdResponse> standards);
        Task UpsertStandardNonApprovedCollations(IEnumerable<GetStandardByIdResponse> standards);
    }
}