using System;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Api.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AuditFilterAttribute : Attribute
    {
        public AuditFilterAttribute(Type auditFilter)
        {
            if(auditFilter.GetInterface(nameof(IAuditFilter)) == null)
                throw new ArgumentException("auditFilter does not implement the interface IAuditFilter");

            AuditFilter = auditFilter;
        }

        public Type AuditFilter { get; set; }
    }
}
