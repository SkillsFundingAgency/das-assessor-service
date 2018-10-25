using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class DuplicateRequestViewModel
    {
        public Certificate Certificate { get; set; }
        public bool IsConfirmed { get; set; }
        public string NextBatchDate { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public long Uln { get; set; }
        public int StdCode { get; set; }
        public bool BackToCheckPage { get; set; }
    }
}