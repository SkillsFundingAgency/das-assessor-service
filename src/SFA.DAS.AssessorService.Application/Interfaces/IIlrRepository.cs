using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln);
        
        Task<Ilr> Get(long uln, int standardCode);
        
        Task StoreSearchLog(SearchLog log);
        
        Task<IEnumerable<Ilr>> Search(string searchQuery);
        
        Task Create(string source, long ukprn, long uln, int stdCode, int? fundingModel, string givenNames, string familyName,
                string epaOrgId, DateTime? learnStartDate, DateTime? plannedEndDate, int? completionStatus, string learnRefNumber, string delLocPostCode,
                DateTime? learnActEndDate, int? withdrawReason, int? outcome, DateTime? achDate, string outGrade);

        Task Update(string source, long ukprn, long uln, int stdCode, int? fundingModel, string givenNames, string familyName,
                string epaOrgId, DateTime? learnStartDate, DateTime? plannedEndDate, int? completionStatus, string learnRefNumber, string delLocPostCode,
                DateTime? learnActEndDate, int? withdrawReason, int? outcome, DateTime? achDate, string outGrade);
    }
}