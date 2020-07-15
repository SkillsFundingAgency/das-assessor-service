using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Data
{
    public class EMailTemplateQueryRepository : Repository, IEMailTemplateQueryRepository
    {
        public EMailTemplateQueryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {            
        }       

        public async Task<EmailTemplateSummary> GetEmailTemplate(string templateName)
        {
            var sql = @"SELECT ET.Id, ET.TemplateName, ET.TemplateId, ETR.Recipients, ET.RecipientTemplate
                    FROM [dbo].[EMailTemplates] ET join [dbo].[EmailTemplatesRecipients] ETR
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