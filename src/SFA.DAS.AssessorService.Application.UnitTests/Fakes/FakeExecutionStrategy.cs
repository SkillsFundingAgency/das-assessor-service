using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SFA.DAS.AssessorService.Application.UnitTests.Fakes
{
    public class FakeExecutionStrategy : IExecutionStrategy
    {
        public bool RetriesOnFailure => false;

        public TResult Execute<TState, TResult>(TState state, Func<DbContext, TState, TResult> operation, Func<DbContext, TState, ExecutionResult<TResult>> verifySucceeded)
        {
            return operation(null, state);
        }

        public Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<DbContext, TState, CancellationToken, Task<TResult>> operation, Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            return operation(null, state, cancellationToken).ContinueWith(task =>
            {
                if (task.IsFaulted)
                    throw task.Exception;

                return task.Result;
            });
        }
    }
}
