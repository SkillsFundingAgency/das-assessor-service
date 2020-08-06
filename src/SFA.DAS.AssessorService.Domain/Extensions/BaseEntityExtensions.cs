using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Domain.Extensions
{
    public static class BaseEntityExtensions
    {
        public static DateTime? LatestChange(this BaseEntity entity)
        {
            return new[] { entity.DeletedAt, entity.UpdatedAt, entity.CreatedAt }.Max();
        }
    }
}
