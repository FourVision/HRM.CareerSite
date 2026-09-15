using Hrm.CareerSite.Configuration;
using Microsoft.Extensions.Options;

namespace Hrm.CareerSite.Services;

/// <summary>
/// Stores and updates the active HRM server connection configuration for the running application instance.
/// </summary>
public sealed class HrmServerRuntimeConfiguration(IOptionsMonitor<HrmServerOptions> optionsMonitor)
{
    private readonly object syncRoot = new();
    private HrmServerOptions currentOptions = optionsMonitor.CurrentValue;

    /// <summary>
    /// Gets the currently active HRM server options.
    /// </summary>
    public HrmServerOptions CurrentOptions
    {
        get
        {
            lock (syncRoot)
            {
                return currentOptions;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether setup mode is enabled.
    /// </summary>
    public bool IsSetupModeEnabled => string.IsNullOrWhiteSpace(CurrentOptions.Authentication.Mode) || CurrentOptions.Authentication.Mode.Equals("None", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Applies API key settings and exits setup mode.
    /// </summary>
    /// <param name="host">Absolute HRM server host URL.</param>
    /// <param name="apiKeyHeaderName">API key header name.</param>
    /// <param name="apiKey">API key value.</param>
    public void ConfigureApiKey(string host, string apiKeyHeaderName, string apiKey)
    {
        ValidateHost(host);

        if (string.IsNullOrWhiteSpace(apiKeyHeaderName))
        {
            throw new ArgumentException("API key header name is required.", nameof(apiKeyHeaderName));
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        SetOptions(new HrmServerOptions
        {
            Host = host.Trim(),
            Authentication = new HrmServerAuthenticationOptions
            {
                Mode = "ApiKey",
                ApiKeyHeaderName = apiKeyHeaderName.Trim(),
                ApiKey = apiKey.Trim(),
                OAuth = new HrmServerOAuthClientCredentialsOptions()
            }
        });
    }

    /// <summary>
    /// Applies OAuth settings and exits setup mode.
    /// </summary>
    /// <param name="host">Absolute HRM server host URL.</param>
    /// <param name="tenantId">Microsoft Entra tenant identifier.</param>
    /// <param name="clientId">OAuth client identifier.</param>
    /// <param name="clientSecret">OAuth client secret.</param>
    public void ConfigureOAuth(string host, string tenantId, string clientId, string clientSecret)
    {
        ValidateHost(host);

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("OAuth tenant ID is required.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("OAuth client ID is required.", nameof(clientId));
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new ArgumentException("OAuth client secret is required.", nameof(clientSecret));
        }

        SetOptions(new HrmServerOptions
        {
            Host = host.Trim(),
            Authentication = new HrmServerAuthenticationOptions
            {
                Mode = "OAuth",
                ApiKeyHeaderName = string.Empty,
                ApiKey = null,
                OAuth = new HrmServerOAuthClientCredentialsOptions
                {
                    TenantId = tenantId.Trim(),
                    ClientId = clientId.Trim(),
                    ClientSecret = clientSecret.Trim()
                }
            }
        });
    }

    private void SetOptions(HrmServerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        lock (syncRoot)
        {
            currentOptions = options;
        }
    }

    private static void ValidateHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException("HRM server host is required.", nameof(host));
        }

        if (!Uri.TryCreate(host, UriKind.Absolute, out _))
        {
            throw new ArgumentException("HRM server host must be an absolute URL.", nameof(host));
        }
    }
}
