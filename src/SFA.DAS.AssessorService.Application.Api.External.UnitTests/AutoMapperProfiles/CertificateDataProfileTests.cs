using System;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using Request = SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using Response = SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.AutoMapperProfiles
{
    public class CertificateDataProfileTests
    {
        private IMapper _sut;

        [SetUp]
        public void SetUp()
        {
            var configuration = new MapperConfiguration(config =>
                config.AddProfile<CertificateDataProfile>());

            _sut = configuration.CreateMapper();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Incoming")]
        public void WhenRequestContainsPostalContact_ThenDeliveryDetailsAreIgnored(
            string incomingValue)
        {
            // Arrange
            var source = CreateRequest();

            source.PostalContact = new Request.PostalContact
            {
                ContactName = incomingValue,
                Organisation = incomingValue,
                Department = incomingValue,
                AddressLine1 = incomingValue,
                AddressLine2 = incomingValue,
                AddressLine3 = incomingValue,
                City = incomingValue,
                PostCode = incomingValue
            };

            // Act
            var result = _sut.Map<CertificateData>(source);

            // Assert
            AssertNoDeliveryDetails(result);
            AssertAssessmentDetails(result, source);
        }

        [Test]
        public void WhenRequestHasNoPostalContact_ThenAssessmentDetailsAreMapped()
        {
            // Arrange
            var source = CreateRequest();
            source.PostalContact = null;

            // Act
            var result = _sut.Map<CertificateData>(source);

            // Assert
            AssertNoDeliveryDetails(result);
            AssertAssessmentDetails(result, source);
        }

        [Test]
        public void WhenResponseIsMapped_ThenPostalContactIsNullAndStoredDataIsUnchanged()
        {
            // Arrange
            var source = CreateStoredData();
            var originalData = JsonConvert.SerializeObject(source);

            // Act
            var result = _sut.Map<Response.CertificateData>(source);

            // Assert
            result.PostalContact.Should().BeNull();

            result.Learner.GivenNames.Should()
                .Be(source.LearnerGivenNames);
            result.Learner.FamilyName.Should()
                .Be(source.LearnerFamilyName);

            result.LearningDetails.OverallGrade.Should()
                .Be(source.OverallGrade);
            result.LearningDetails.Version.Should()
                .Be(source.Version);
            result.LearningDetails.AchievementDate.Should()
                .Be(source.AchievementDate);

            JsonConvert.SerializeObject(source).Should()
                .Be(originalData);
        }

        [Test]
        public void WhenMappingIntoExistingResponse_ThenPreviousPostalContactIsCleared()
        {
            // Arrange
            var source = CreateStoredData();

            var destination = new Response.CertificateData
            {
                PostalContact = new Response.PostalContact
                {
                    ContactName = "Previous recipient",
                    AddressLine1 = "Previous address",
                    PostCode = "CV1 2WT"
                }
            };

            // Act
            _sut.Map(source, destination);

            // Assert
            destination.PostalContact.Should().BeNull();
            source.ContactName.Should().Be("Existing recipient");
            source.ContactAddLine1.Should().Be("1 Existing Road");
        }

        [Test]
        public void WhenResponseIsSerialized_ThenPostalContactIsExplicitlyNull()
        {
            // Arrange
            var source = CreateStoredData();
            var response = _sut.Map<Response.CertificateData>(source);

            var settings = new JsonSerializerSettings
            {
                ContractResolver =
                    new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling =
                    DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Include
            };

            // Act
            var json = JObject.Parse(
                JsonConvert.SerializeObject(response, settings));

            // Assert
            var postalContact = json.Property("postalContact");

            postalContact.Should().NotBeNull();
            postalContact.Value.Type.Should().Be(JTokenType.Null);
        }

        private static Request.CertificateData CreateRequest()
        {
            return new Request.CertificateData
            {
                Standard = new Request.Standard
                {
                    StandardReference = "ST0123"
                },
                Learner = new Request.Learner
                {
                    FamilyName = "Smith",
                    Uln = 1234567890
                },
                LearningDetails = new Request.LearningDetails
                {
                    Version = "1.0",
                    OverallGrade = CertificateGrade.Pass,
                    CourseOption = "Example option",
                    AchievementDate = new DateTime(
                        2021, 7, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                CoronationEmblem = true
            };
        }

        private static CertificateData CreateStoredData()
        {
            return new CertificateData
            {
                LearnerGivenNames = "Alex",
                LearnerFamilyName = "Smith",
                StandardReference = "ST0123",
                StandardName = "Example standard",
                StandardLevel = 3,
                Version = "1.0",
                OverallGrade = CertificateGrade.Pass,
                AchievementDate = new DateTime(
                    2021, 7, 1, 0, 0, 0, DateTimeKind.Utc),

                SendTo = CertificateSendTo.Apprentice,
                ContactName = "Existing recipient",
                ContactOrganisation = "Existing organisation",
                Department = "Existing department",
                ContactAddLine1 = "1 Existing Road",
                ContactAddLine2 = "Existing building",
                ContactAddLine3 = "Existing district",
                ContactAddLine4 = "Existing city",
                ContactPostCode = "CV1 2WT"
            };
        }

        private static void AssertNoDeliveryDetails(
            CertificateData result)
        {
            result.SendTo.Should().Be(CertificateSendTo.None);
            result.ContactName.Should().BeNull();
            result.ContactOrganisation.Should().BeNull();
            result.Department.Should().BeNull();
            result.ContactAddLine1.Should().BeNull();
            result.ContactAddLine2.Should().BeNull();
            result.ContactAddLine3.Should().BeNull();
            result.ContactAddLine4.Should().BeNull();
            result.ContactPostCode.Should().BeNull();
        }

        private static void AssertAssessmentDetails(
            CertificateData result,
            Request.CertificateData source)
        {
            result.LearnerFamilyName.Should()
                .Be(source.Learner.FamilyName);
            result.StandardReference.Should()
                .Be(source.Standard.StandardReference);
            result.AchievementDate.Should()
                .Be(source.LearningDetails.AchievementDate);
            result.CourseOption.Should()
                .Be(source.LearningDetails.CourseOption);
            result.Version.Should()
                .Be(source.LearningDetails.Version);
            result.OverallGrade.Should()
                .Be(source.LearningDetails.OverallGrade);
            result.CoronationEmblem.Should()
                .Be(source.CoronationEmblem);
        }
    }
}