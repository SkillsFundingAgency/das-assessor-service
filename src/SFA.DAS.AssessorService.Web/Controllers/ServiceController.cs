using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Web.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.AssessorService.Web.Controllers;

[Route("[controller]")]
public class ServiceController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IStubAuthenticationService _stubAuthenticationService;

    public ServiceController(IConfiguration configuration, IStubAuthenticationService stubAuthenticationService)
    {
        _configuration = configuration;
        _stubAuthenticationService = stubAuthenticationService;
    }

    
    [HttpGet]
    [Route("account-details", Name = RouteNames.StubAccountDetailsGet)]
    public IActionResult AccountDetails([FromQuery]string returnUrl)
    {
        if (_configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }
        return View("AccountDetails",new StubAuthenticationViewModel
        {
            ReturnUrl = returnUrl
        });
    }
    [HttpPost]
    [Route("account-details", Name = RouteNames.StubAccountDetailsPost)]
    public async Task<IActionResult> AccountDetails(StubAuthenticationViewModel model)
    {
        if (_configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }

        var claims = await _stubAuthenticationService.GetStubSignInClaims(model);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
            new AuthenticationProperties());
        
        return RedirectToRoute(RouteNames.StubSignedIn, new {returnUrl = model.ReturnUrl});
    }

    [HttpGet]
    [Authorize]
    [Route("Stub-Auth", Name = RouteNames.StubSignedIn)]
    public IActionResult StubSignedIn([FromQuery]string returnUrl) 
    {
        if (_configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }
        var viewModel = new AccountStubViewModel
        {
            Email = User.Claims.FirstOrDefault(c=>c.Type.Equals(ClaimTypes.Email))?.Value,
            Id = User.Claims.FirstOrDefault(c=>c.Type.Equals(ClaimTypes.NameIdentifier))?.Value,
            ReturnUrl = returnUrl
        };
        return View(viewModel);
    }
}