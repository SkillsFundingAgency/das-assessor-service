using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Data
{
    public class Repository
    {
        protected readonly IUnitOfWork _unitOfWork = null;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
