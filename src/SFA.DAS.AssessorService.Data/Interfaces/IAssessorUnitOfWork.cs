using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IAssessorUnitOfWork
    {
        IAssessorDbContext AssessorDbContext { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default);
    }
}
