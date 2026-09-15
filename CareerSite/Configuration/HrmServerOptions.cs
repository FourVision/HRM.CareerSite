using System.ComponentModel.DataAnnotations;

namespace Hrm.CareerSite.Configuration;

/// <summary>
/// Represents the top-level configuration for connecting to the upstream HRM server.
/// </summary>
public sealed class HrmServerOptions
{
    /// <summary>
    /// Gets the configuration section name used for HRM server settings.
    /// </summary>
    public const string SectionName = "HrmServer";

    /// <summary>
    /// Gets the absolute base URL of the upstream HRM server.
    /// </summary>
    public string Host { get; init; } = string.Empty;

    /// <summary>
    /// Gets authentication settings for requests to the upstream HRM server.
    /// </summary>
    public HrmServerAuthenticationOptions Authentication { get; init; } = new();
}

/// <summary>
/// Represents authentication configuration used for HRM server requests.
/// </summary>
public sealed class HrmServerAuthenticationOptions
{
    /// <summary>
    /// Gets the authentication mode. Supported values are <c>ApiKey</c>, <c>OAuth</c>, and <c>None</c>.
    /// </summary>
    [Required]
    [RegularExpression("^(ApiKey|OAuth|None)$")]
    public string Mode { get; init; } = "None";

    /// <summary>
    /// Gets the HTTP header name used to send the API key when <c>ApiKey</c> mode is enabled.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string ApiKeyHeaderName { get; init; } = "X-Api-Key";

    /// <summary>
    /// Gets the API key value used when <c>ApiKey</c> mode is enabled.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets OAuth client credential settings used when <c>OAuth</c> mode is enabled.
    /// </summary>
    public HrmServerOAuthClientCredentialsOptions OAuth { get; init; } = new();
}

/// <summary>
/// Represents OAuth client credentials used to obtain access tokens.
/// </summary>
public sealed class HrmServerOAuthClientCredentialsOptions
{
    /// <summary>
    /// Gets the Microsoft Entra tenant identifier.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets the OAuth client identifier.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Gets the OAuth client secret.
    /// </summary>
    public string? ClientSecret { get; init; }
}
