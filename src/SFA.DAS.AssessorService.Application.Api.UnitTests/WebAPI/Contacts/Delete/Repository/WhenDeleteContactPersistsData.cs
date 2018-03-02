namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Put.Repository
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Linq;

    [Subject("AssessorService")]
    public class WhenDeleteContactPersistsData
    {
        private static ContactRepository _contactRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;      
        private static Mock<DbSet<Contact>> _contactDBSetMock;

        protected static Task _result;


        Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _contactDBSetMock = new Mock<DbSet<Contact>>();

            var mockSet = new Mock<DbSet<Contact>>();
            var mockContext = new Mock<AssessorDbContext>();


            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()              
                .Build()
            }.AsQueryable();

            mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(contacts.Provider);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(contacts.Expression);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(contacts.ElementType);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(contacts.GetEnumerator());

            mockContext.Setup(c => c.Contacts).Returns(mockSet.Object);

            _assessorDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            _assessorDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Contact>()));


            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _contactRepository = new ContactRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _contactRepository.Delete("1234");
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var taskresult = _result as Task;
            taskresult.Should().NotBeNull();
        };
    }
}


