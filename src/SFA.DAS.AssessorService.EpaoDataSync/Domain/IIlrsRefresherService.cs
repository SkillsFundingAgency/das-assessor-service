using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoDataSync.Domain
{
    public interface IIlrsRefresherService
    {
       Task UpdateIlRsTable(string sinceTime);
       Task UpdateIlRsTable();
    }
}
