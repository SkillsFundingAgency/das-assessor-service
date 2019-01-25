using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.AssessorService.Web.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = PolicyTagHelperAttributeName)]
    [HtmlTargetElement("a", Attributes = PolicyTagHelperAttributeName)]
    public class PolicyNotExistTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public PolicyNotExistTagHelper(IHttpContextAccessor contextAccessor, IAuthorizationService authorizationService)
        {
            _contextAccessor = contextAccessor;
            _authorizationService = authorizationService;
        }

        private const string PolicyTagHelperAttributeName = "sfa-policy-not-exists-show";

        [HtmlAttributeName(PolicyTagHelperAttributeName)]
        public string PolicyName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var allowed = _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, PolicyName).Result;
            if (allowed.Succeeded)
            {
                output.SuppressOutput();
            }
        }
    }
}