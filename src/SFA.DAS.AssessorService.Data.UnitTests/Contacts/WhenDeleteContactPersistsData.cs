using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Contacts
{
    public class WhenDeleteContactPersistsData
    {
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();

        [SetUp]
        public async Task Arrange()
        {
            var contacts = new List<Contact>
            {
                Builder<Contact>.CreateNew()    
                  .With(q => q.Username = "1234")
                    .Build()
            }.AsQueryable();

            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(q => q.Contacts).Returns(contacts.CreateMockSet().Object);
            
            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork.Setup(p => p.AssessorDbContext).Returns(mockDbContext.Object);

            var contactRepository = new ContactRepository(_mockAssessorUnitOfWork.Object, new Mock<IUnitOfWork>().Object);

            await contactRepository.Delete("1234");
        }

        [Test]
        public void ThenDeleteOrganisationShouldSucceed()
        {
            _mockAssessorUnitOfWork.Verify(q => q.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}


