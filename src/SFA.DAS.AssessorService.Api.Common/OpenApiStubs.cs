// Minimal OpenAPI model stubs to restore build after package upgrades.
// These are lightweight placeholders mirroring only the members used by the codebase.
namespace Microsoft.OpenApi.Models
{
    using System.Collections.Generic;

    public class OpenApiInfo
    {
        public string Title { get; set; }
        public string Version { get; set; }
    }

    public class OpenApiSecurityScheme
    {
        public ParameterLocation In { get; set; }
        public string Name { get; set; }
        public SecuritySchemeType Type { get; set; }
        public string Description { get; set; }
    }

    public enum ParameterLocation
    {
        Query,
        Header,
        Path,
        Cookie
    }

    public enum SecuritySchemeType
    {
        ApiKey,
        Http,
        OAuth2,
        OpenIdConnect
    }

    public class OpenApiSecurityRequirement : Dictionary<OpenApiSecurityScheme, IEnumerable<string>>
    {
    }

    public class OpenApiSchema
    {
        public IDictionary<string, OpenApiSchema> Properties { get; set; }
        public ISet<string> Required { get; set; }
    }
}
