﻿using System.Threading;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateRequestIsSent : RegisterUpdateTestsBase
    {
        [Test]
        public void ThenItShouldGetAllEpaosFromTheRegister()
        {
            Setup();
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            ApiClient.Verify(client => client.FindAllAsync());
        }

        [Test]
        public void ThenItShouldGetAllCurrentOrganisationsFromTheRepository()
        {
            Setup();
            RegisterUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
            OrganisationRepository.Verify(r => r.GetAllOrganisations());
        }
    }
}