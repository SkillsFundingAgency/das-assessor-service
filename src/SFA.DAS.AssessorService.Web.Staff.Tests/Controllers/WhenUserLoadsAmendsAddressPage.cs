using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers
{
    public class WhenUserLoadsAmendsAddressPage : ContractAmendQueryBase
    {     
        private IActionResult _result;
        private CertificateAddressViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificateAddressController = new CertificateAddressController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient);
            _result = certificateAddressController.Address(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateAddressViewModel;
        }

        [Test]
        public void ThenShouldReturnValidAdddressLine1()
        {         
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.AddressLine1.Should().Be(certificateData.ContactAddLine1);           
        }

        [Test]
        public void ThenShouldReturnValidAdddressLine2()
        {                      
            _viewModelResponse.AddressLine2.Should().Be(CertificateData.ContactAddLine2);
       }

        [Test]
        public void ThenShouldReturnValidAdddressLine3()
        {                       
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.AddressLine3.Should().Be(CertificateData.ContactAddLine3);            
        }
        [Test]
        public void ThenShouldReturnValidCity()
        {                     
            _viewModelResponse.City.Should().Be(CertificateData.ContactAddLine4);            
        }

        [Test]
        public void ThenShouldReturnValidPostcode()
        {                     
            _viewModelResponse.Postcode.Should().Be(CertificateData.ContactPostCode);
        }
    }   
}
