namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller.Put.Repository
{
    using FizzWare.NBuilder;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenUpdateContactPersistsData
    {
        private static ContactRepository _contactRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static UpdateContactRequest _contactUpdateViewModel;
        private static Mock<DbSet<Contact>> _contactDBSetMock;

        protected static Contactl _result;


        Establish context = () =>
        {
            Bootstrapper.Initialize();

            _contactUpdateViewModel = Builder<UpdateContactRequest>.CreateNew().Build();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _contactDBSetMock = new Mock<DbSet<Contact>>();

            var mockSet = new Mock<DbSet<Contact>>();
            var mockContext = new Mock<AssessorDbContext>();


            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew().Build()
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
            _contactRepository.Update(_contactUpdateViewModel);
        };

        Machine.Specifications.It verify_succesfully = () =>
        {          
        };
    }
}

