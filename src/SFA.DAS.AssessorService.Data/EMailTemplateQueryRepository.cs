using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class EMailTemplateQueryRepository : IEMailTemplateQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public EMailTemplateQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<EMailTemplate> GetEMailTemplate(string templateName)
        {
            return await _assessorDbContext.EMailTemplates.FirstOrDefaultAsync(q => q.TemplateName == templateName);
        }
    }
}