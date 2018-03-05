using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers
{
    public static class EntityHelpers
    {
        public static Mock<DbSet<T>> CreateMockSet<T>(this IQueryable data, IQueryable<T> organisations)
            where T : BaseEntity
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(organisations.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(organisations.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(organisations.GetEnumerator());
            return mockSet;
        }
    }
}
