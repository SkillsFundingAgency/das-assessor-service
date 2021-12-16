using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenGetOrganisationEarliestWithdrawalDateIsCalled : OrganisationQueryBase
    {
        private Guid _epaoId;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _epaoId = Guid.NewGuid();
        }

        [Test]
        public async Task With_StandardId_Then_QuerySent()
        {
            var standardId = Builder<int>.CreateNew().Build();

            await OrganisationQueryController.GetOrganisationStandardEarliestWithdrawal(_epaoId, standardId);

            Mediator.Verify(m => m.Send(It.Is<GetEarliestWithdrawalDateRequest>(q => q.OrganisationId == _epaoId
                && q.StandardId == standardId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Without_StandardId_Then_QuerySent()
        {
            await OrganisationQueryController.GetOrganisationEarliestWithdrawal(_epaoId);

            Mediator.Verify(m => m.Send(It.Is<GetEarliestWithdrawalDateRequest>(q => q.OrganisationId == _epaoId
                && q.StandardId == null), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
