using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.FrameworkLearners
{
    public class WhenSystemSearchesFrameworkLearnersByFirstNameAndSurnameAndDateOfBirth
    {
        private FrameworkLearnerRepository _frameworkLearnerRepository;
        private Mock<IAssessorUnitOfWork> _mockAssessorUnitOfWork;
        private Mock<AssessorDbContext> _mockDbContext;
        private List<FrameworkLearner> _results;
        private string _firstName = "Jamal";
        private string _lastName = "Franklin";
        private DateTime _dateOfBirth = DateTime.Now.AddYears(-20);

        [SetUp]
        public void Arrange()
        {            
            _mockDbContext = CreateMockDbContext();
            _mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockAssessorUnitOfWork
                .SetupGet(p => p.AssessorDbContext)
                .Returns(_mockDbContext.Object);

            _frameworkLearnerRepository = new FrameworkLearnerRepository(_mockAssessorUnitOfWork.Object);

            _results = _frameworkLearnerRepository.Search(_firstName, _lastName, _dateOfBirth).Result.ToList();
        }

        [Test]
        public void ItShouldReturnResults()
        {
            _results.Should().HaveCount(2);
            _results.Should().AllSatisfy(learner =>
            {
                learner.ApprenticeForename.Should().Be(_firstName);
                learner.ApprenticeSurname.Should().Be(_lastName);
                learner.ApprenticeDoB.Should().Be(_dateOfBirth);
                learner.CertificationYear.Should().Be("2002");
            });
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var frameworkLearners = Builder<FrameworkLearner>.CreateListOfSize(10)
                .TheFirst(2)
                .With(x => x.ApprenticeForename = _firstName)
                .With(x => x.ApprenticeSurname = _lastName)
                .With(x => x.ApprenticeDoB = _dateOfBirth)
                .With (x => x.CertificationYear = "2002")
                .TheNext(8)
                .Build()
                .AsQueryable();

            mockDbContext.Setup(x => x.FrameworkLearners).ReturnsDbSet(frameworkLearners);

            return mockDbContext;
        }
    }
}