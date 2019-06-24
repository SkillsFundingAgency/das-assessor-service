using System;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Attributes
{
    /// <summary>
    /// The class purely marks something in swagger as Required.
    /// It does not replicate the behaviour found in System.ComponentModel.Annotations.RequiredAttibute
    /// </summary>
    public class SwaggerRequiredAttribute : Attribute
    {
    }
}
