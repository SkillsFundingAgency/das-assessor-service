using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    public class ApplicationDataHandler : SqlMapper.TypeHandler<ApplicationData>
    {
        public override void SetValue(IDbDataParameter parameter, ApplicationData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override ApplicationData Parse(object value)
        {
            return JsonConvert.DeserializeObject<ApplicationData>(value.ToString());
        }

    }
}
