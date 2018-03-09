//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using FizzWare.NBuilder;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.AssessorService.Api.Types.Models;
//using SFA.DAS.AssessorService.Data;
//using SFA.DAS.AssessorService.Domain.Entities;

//namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Put.Repository
//{
//    public class WhenUpdateContactPersistsData
//    {
//        private static ContactRepository _contactRepository;
//        private static Mock<AssessorDbContext> _assessorDbContext;
//        private static UpdateContactRequest _updateContactRequest;
//        private static Mock<DbSet<Contact>> _contactDBSetMock;

//        protected static ContactResponse _result;

//        [SetUp]
//        public async Task Arrange()
//        {
//            MappingBootstrapper.Initialize();

//            _updateContactRequest = Builder<UpdateContactRequest>.CreateNew().Build();

//            _assessorDbContext = new Mock<AssessorDbContext>();
//            _contactDBSetMock = new Mock<DbSet<Contact>>();

//            var mockSet = new Mock<DbSet<Contact>>();

//            var contacts = new List<Contact>
//            {
//                Builder<Contact>.CreateNew()
//                    .With(x => x.Username = _updateContactRequest.Username)
//                    .Build()
//            }.AsQueryable();

//            mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(contacts.Provider);
//            mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(contacts.Expression);
//            mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(contacts.ElementType);
//            mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(contacts.GetEnumerator());

//            _assessorDbContext.Setup(c => c.Contacts).Returns(mockSet.Object);
//            _assessorDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Contact>()));


//            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
//                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

//            _contactRepository = new ContactRepository(_assessorDbContext.Object);

//            _contactRepository.Update(_updateContactRequest);
//        }

//        [Test]
//        public void ThenItShouldSucceed()
//        {
//            _assessorDbContext.Verify(q => q.SaveChangesAsync(new CancellationToken()), Times.Once());
//        }
//    }
//}

