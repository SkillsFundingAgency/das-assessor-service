﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.OrganisationContoller.Put.Repository
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Application.OrganisationHandlers;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Linq;

    [Subject("AssessorService")]
    public class WhenDeletePersistsData
    {
        private static OrganisationRepository _organisationRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static OrganisationUpdateDomainModel _organisationUpdateDomainModel;
        private static Mock<DbSet<Organisation>> _organisationDBSetMock;

        protected static Task _result;


        Establish context = () =>
        {
            Bootstrapper.Initialize();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _organisationDBSetMock = new Mock<DbSet<Organisation>>();

            var mockSet = new Mock<DbSet<Organisation>>();
            var mockContext = new Mock<AssessorDbContext>();


            var organisations = new List<Organisation>
            {
                Builder<Organisation>.CreateNew()
                .With(q => q.EndPointAssessorUKPRN = 10000000)
                .Build()
            }.AsQueryable();

            mockSet.As<IQueryable<Organisation>>().Setup(m => m.Provider).Returns(organisations.Provider);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.Expression).Returns(organisations.Expression);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.GetEnumerator()).Returns(organisations.GetEnumerator());

            mockContext.Setup(c => c.Organisations).Returns(mockSet.Object);

            _assessorDbContext.Setup(q => q.Organisations).Returns(mockSet.Object);
            _assessorDbContext.Setup(x => x.MarkAsModified(Moq.It.IsAny<Organisation>()));


            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _organisationRepository = new OrganisationRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _organisationRepository.Delete(Guid.NewGuid());
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            var taskresult = _result as Task;
            taskresult.Should().NotBeNull();
        };
    }
}


