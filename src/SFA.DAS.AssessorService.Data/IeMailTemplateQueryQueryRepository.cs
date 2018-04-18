using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class EMailTemplateQueryQueryRepository : IEMailTemplateQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public EMailTemplateQueryQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<List<EMailTemplate>> GetEMailTemplates()
        {
            return await _assessorDbContext.EMailTemplates.ToListAsync();
        }
    }
}