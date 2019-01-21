﻿using FizzWare.NBuilder;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter.Const;

namespace SFA.DAS.AssessorService.PrintFunction.Tests.AssessorServiceApi
{
    public class WhenSystemGetsEmailTemplate : AssessorServiceTestBase
    {
        private EMailTemplate _result;
        private EMailTemplate _emailTemplate;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _emailTemplate = Builder<EMailTemplate>.CreateNew().Build();

            var templateName = EMailTemplateNames.PrintAssessorCoverLetters;
            MockHttp.When($"http://localhost:59022/api/v1/emailTemplates/{templateName}")
                .Respond("application/json", JsonConvert.SerializeObject(_emailTemplate)); // Respond with JSON

            _result = AssessorServiceApi.GetEmailTemplate(templateName).Result;
        }

        [Test]
        public void ThenItShouldReturnAnEmailTemplate()
        {
            _result.Should().BeOfType<EMailTemplate>();
        }
    }
}
