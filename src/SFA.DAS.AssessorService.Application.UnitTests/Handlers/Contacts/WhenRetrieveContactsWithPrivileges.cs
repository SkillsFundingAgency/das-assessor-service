using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts
{
    public class WhenRetrieveContactsWithPrivileges
    {
        private List<ContactsWithPrivilegesResponse> _result;

        [SetUp]
        public void Arrange()
        {
         
            var contactQueryRepositoryMock = new Mock<IContactQueryRepository>();

            var list = new List<ContactsPrivilege>
            {
                new ContactsPrivilege()
                {
                    Contact = Builder<Contact>.CreateNew().Build(),
                    ContactId = Guid.NewGuid(),
                    Privilege = Builder<Privilege>.CreateNew().Build()
                },
                new ContactsPrivilege()
                {
                    Contact = Builder<Contact>.CreateNew().Build(),
                    ContactId = Guid.NewGuid(),
                    Privilege = Builder<Privilege>.CreateNew().Build()
                }
            };

            var listOfGroupedContactsWithPrivileges = list.GroupBy(p => p.Contact);

            contactQueryRepositoryMock.Setup(x => x.GetAllContactsWithPrivileges(It.IsAny<string>()))
                .Returns(Task.FromResult(listOfGroupedContactsWithPrivileges));
            var getContactsRequest = new GetContactsWithPrivilagesRequest("organisationId");
            var retrieveContactsHandler = new RetrieveContactsWithPrivilagesHandler(contactQueryRepositoryMock.Object);
            _result = retrieveContactsHandler.Handle(getContactsRequest,new CancellationToken()).Result;
        }

        [Test]
        public void Should_Return_Contacts_With_Privileges()
        {
            _result.Should().NotBeNull();
            _result.Should().HaveCount(2);
            _result.Should().OnlyHaveUniqueItems();
            var itemResult = _result.FirstOrDefault();
            itemResult?.Should().NotBeNull();
            itemResult?.Contact.Should().NotBeNull();
            itemResult?.Privileges.Should().NotBeNull();
            itemResult?.Privileges.Should().HaveCountGreaterThan(0);
        }
    }
}
