using System.Data;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection { get; private set; } = null;

        public IDbTransaction Transaction { get; private set; } = null;

        public void Begin()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            Transaction.Rollback();
            Dispose();
        }

        public void Dispose()
        {
            if (Transaction != null)
                Transaction.Dispose();

            if (Connection.State != ConnectionState.Closed)
                Connection.Close();

            Transaction = null;
        }
    }
}
