using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;


namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IAssessorUnitOfWork
    {
        IAssessorDbContext AssessorDbContext { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default);
		Task<int> ExecuteStoredProcedureAsync(string procName, DynamicParameters parameters = null, int? commandTimeout = null, CancellationToken cancellationToken = default);
		Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procName, DynamicParameters parameters = null, int? commandTimeout = null, CancellationToken cancellationToken = default);
	}
}
