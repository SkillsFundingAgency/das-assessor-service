using Dapper;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Linq;

namespace SFA.DAS.AssessorService.Data
{
    public class EMailTemplateQueryRepository : Repository, IEMailTemplateQueryRepository
    {
        public EMailTemplateQueryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<EmailTemplateSummary> GetEmailTemplate(string templateName)
        {
            var sql = @"SELECT ET.Id, ET.TemplateName, ET.TemplateId, ETR.Recipients
                    FROM [dbo].[EMailTemplates] ET left join [dbo].[EmailTemplatesRecipients] ETR
                    ON ET.Id = ETR.EmailTemplateId
                    WHERE ET.TemplateName = @templateName";

            var results = await _unitOfWork.Connection.QueryAsync<EmailTemplateSummary>(
                sql,
                param: new { templateName },
                transaction: _unitOfWork.Transaction);

            return results.FirstOrDefault();
        }
    }
}