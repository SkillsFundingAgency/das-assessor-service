﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller.Post.Handlers
{
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SFA.DAS.AssessorService.Application.ContactHandlers;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Data;
    using SFA.DAS.AssessorService.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Contact = AssessorService.Api.Types.Models.Contact;

    [Subject("AssessorService")]
    public class WhenCreateContactPersistsData
    {
        private static ContactRepository _contactRepository;
        private static Mock<AssessorDbContext> _assessorDbContext;
        private static ContactCreateDomainModel _contactCreateDomainModel;
        private static Mock<DbSet<AssessorService.Domain.Entities.Contact>> _contactDBSetMock;        
        protected static Contact _result;
        
        Establish context = () =>
        {
            MappingBootstrapper.Initialize();

            _contactCreateDomainModel = Builder<ContactCreateDomainModel>.CreateNew().Build();

            _assessorDbContext = new Mock<AssessorDbContext>();
            _contactDBSetMock = new Mock<DbSet<AssessorService.Domain.Entities.Contact>>();

            var mockSet = new Mock<DbSet<AssessorService.Domain.Entities.Contact>>();
            var mockContext = new Mock<AssessorDbContext>();

            var organisations = new List<AssessorService.Domain.Entities.Contact>();

            mockSet.Setup(m => m.Add(Moq.It.IsAny< AssessorService.Domain.Entities.Contact>())).Callback((AssessorService.Domain.Entities.Contact organisation) => organisations.Add(organisation));

            _assessorDbContext.Setup(q => q.Contacts).Returns(mockSet.Object);
            _assessorDbContext.Setup(q => q.SaveChangesAsync(new CancellationToken()))
                .Returns(Task.FromResult((Moq.It.IsAny<int>())));

            _contactRepository = new ContactRepository(_assessorDbContext.Object);

        };

        Because of = () =>
        {
            _result = _contactRepository.CreateNewContact(_contactCreateDomainModel).Result;
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            _result.Should().NotBeNull();
        };
    }
}

