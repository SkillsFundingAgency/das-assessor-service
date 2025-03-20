using System.Collections.Generic;
using SFA.DAS.AssessorService.Data.Interfaces;

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

    public class SqlQuery
    {
        public const string PredicatePlaceholder = "<<PREDICATE>>";

        public string Sql { get; private set; }
        public List<string> Predicates { get; } = new List<string>();

        public SqlQuery(string sql, string predicate = null)
        {
            Sql = sql;

            if (!string.IsNullOrEmpty(predicate))
                Predicates.Add(predicate);
        }

        public string SqlWithOptionalPredicates()
        {
            string sqlPredicates = Predicates.Count > 0 
                ? $"WHERE {string.Join(" AND ", Predicates.ToArray())}" 
                : string.Empty;

            return Sql.Replace(PredicatePlaceholder, sqlPredicates);
        }
    }
}
