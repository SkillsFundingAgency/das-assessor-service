using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query;

public class WhenGettingContactByGovIdentifier : ContactsQueryBase
{
    private IActionResult _result;
    private string _govIdentifier = "some-randomIdenfifier123avs1234";
    private Contact _contact;

    [SetUp]
    public void Arrange()
    {
        Setup();
        MappingBootstrapper.Initialize();
        _contact = Builder<Contact>.CreateNew().Build();

        ContactQueryRepositoryMock.Setup(q => q.GetContactFromGovIdentifier(_govIdentifier))
            .ReturnsAsync(_contact);

    }

    [Test]
    public async Task ThenTheResultReturnsOkStatus()
    {
        _result = await ContactQueryController.SearchContactByGovIdentifier(_govIdentifier);
            
        var result = _result as OkObjectResult;
        result.Should().NotBeNull();
        var actualModel = result!.Value as ContactResponse;
        actualModel.Should().NotBeNull();
        actualModel.Should().BeEquivalentTo(_contact, options=> options.ExcludingMissingMembers());
    }

    [Test]
    public void Then_If_Null_Returned_Exception_Thrown()
    {
        Assert.ThrowsAsync<ResourceNotFoundException>(()=> ContactQueryController.SearchContactByGovIdentifier("some-other-value"));
    }
}