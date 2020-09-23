using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Data;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class GovernanceRecommendationHandler : SqlMapper.TypeHandler<GovernanceRecommendation>
    {
        public override void SetValue(IDbDataParameter parameter, GovernanceRecommendation value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override GovernanceRecommendation Parse(object value)
        {
            return JsonConvert.DeserializeObject<GovernanceRecommendation>(value.ToString());
        }

    }
}

