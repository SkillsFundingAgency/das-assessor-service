using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Helpers;

namespace SFA.DAS.AssessorService.Web.Models;

public class SignedOutViewModel
{
    private readonly string _environment;

    public SignedOutViewModel(string environment)
    {
        _environment = environment;
    }
    public string ServiceLink =>   $"https://{EnvironmentHelper.GetDomain(_environment)}";
}