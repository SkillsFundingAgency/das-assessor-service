using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Data;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class FinancialGradeHandler : SqlMapper.TypeHandler<FinancialGrade>
    {
        public override void SetValue(IDbDataParameter parameter, FinancialGrade value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override FinancialGrade Parse(object value)
        {
            return JsonConvert.DeserializeObject<FinancialGrade>(value.ToString());
        }

    }


    public class FinancialEvidenceHandler : SqlMapper.TypeHandler<FinancialEvidence>
    {
        public override void SetValue(IDbDataParameter parameter, FinancialEvidence value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override FinancialEvidence Parse(object value)
        {
            return JsonConvert.DeserializeObject<FinancialEvidence>(value.ToString());
        }

    }
}

