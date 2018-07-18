using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;


namespace SFA.DAS.AssessorService.Web.Staff.Tests
{
    //[TestFixture]
    //public class StaffDashboardTests 
    //{
    //    private HttpClient _httpClient;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        _httpClient = new WebApplicationFactory<Startup>().CreateClient(
    //            new WebApplicationFactoryClientOptions() {BaseAddress = new Uri("https://localhost:44347")});
    //    }

    //    [Test]
    //    public async Task HomeReturns200()
    //    {
    //        var response = await _httpClient.GetAsync("/");
    //        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    //    }

    //    [Test]
    //    public async Task AccountSignInReturnsRedirect()
    //    {
    //        var response = await _httpClient.GetAsync("/Dashboard/Index");
    //        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    //    }
    //}
}
