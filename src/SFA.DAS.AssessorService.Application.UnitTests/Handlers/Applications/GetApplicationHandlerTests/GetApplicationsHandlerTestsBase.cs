using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    [TestFixture]
    public class GetApplicationsHandlerTestsBase : MapperBase
    {
        protected Mock<IApplyRepository> ApplyRepository;
        protected GetApplicationsHandler Handler;

        [SetUp]
        public void Setup()
        {
            ApplyRepository = new Mock<IApplyRepository>();
            Handler = new GetApplicationsHandler(ApplyRepository.Object, Mapper);
        }
    }
}