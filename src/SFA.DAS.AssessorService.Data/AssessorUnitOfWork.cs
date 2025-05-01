using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using System;
using System.Threading;
using Dapper;
using System.Data;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessorUnitOfWork : IAssessorUnitOfWork
    {
        private readonly IAssessorDbContext _context;
        private IDbContextTransaction _transaction;

        public IAssessorDbContext AssessorDbContext => _context;

        public AssessorUnitOfWork(IAssessorDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                T result;

                try
                {
                    await BeginTransactionAsync(cancellationToken);

                    result = await func(); 

                    await CommitTransactionAsync(cancellationToken);
                }
                catch
                {
                    await RollbackTransactionAsync(cancellationToken);
                    throw;
                }

                return result;
            });
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await BeginTransactionAsync(cancellationToken);

                    await action();

                    await CommitTransactionAsync(cancellationToken);
                }
                catch
                {
                    await RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            });
        }

		public async Task<int> ExecuteStoredProcedureAsync(string procName, DynamicParameters parameters = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
		{
			var connection = _context.Database.GetDbConnection();
			var transaction = _transaction?.GetDbTransaction();
			if (connection.State != ConnectionState.Open)
				await connection.OpenAsync(cancellationToken);
			return await connection.ExecuteAsync(
			  procName,
			  param: parameters,
			  transaction: transaction,
			  commandTimeout: commandTimeout,
			  commandType: CommandType.StoredProcedure);
		}
		public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procName, DynamicParameters parameters = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
		{
			var connection = _context.Database.GetDbConnection();
			var transaction = _transaction?.GetDbTransaction();
			if (connection.State != ConnectionState.Open)
				await connection.OpenAsync(cancellationToken);
			return await connection.QueryAsync<T>(
			  sql: procName,
			  param: parameters,
			  transaction: transaction,
			  commandTimeout: commandTimeout,
			  commandType: CommandType.StoredProcedure);
		}

		private async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }
        }

        private async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        private async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
