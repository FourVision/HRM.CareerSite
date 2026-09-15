using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hrm.CareerSite.Configuration;
using Hrm.CareerSite.Models;

namespace Hrm.CareerSite.Services;

/// <summary>
/// Provides access to job listing data from the upstream HRM server.
/// </summary>
public sealed class HrmServerClient(HttpClient httpClient, HrmServerRuntimeConfiguration runtimeConfiguration)
{
    private const string RecruitmentsPath = "/api/data/RecruitmentProjects/FilterExternalJobOpenings";
    private const string OAuthScope = "api://19d62800-253f-4ad0-9d7e-243c57e2b982/.default";

    private static readonly JsonSerializerOptions DefaultSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        MaxDepth = 256,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateOnlyJsonConverter(),
            new NullableDateOnlyJsonConverter()
        }
    };

    /// <summary>
    /// Gets jobs from the configured HRM upstream server.
    /// </summary>
    /// <param name="filterObject">The filter payload sent to the upstream endpoint.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>A read-only collection of matching job listings.</returns>
    public async Task<IReadOnlyList<JobListing>> GetJobsAsync(DataFilterObject filterObject, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filterObject);

        HrmServerOptions options = runtimeConfiguration.CurrentOptions;
        using HttpRequestMessage request = new(HttpMethod.Post, BuildUrl(options, RecruitmentsPath))
        {
            Content = JsonContent.Create(filterObject, options: DefaultSerializerOptions)
        };

        await AddAuthorizationAsync(request, options, cancellationToken).ConfigureAwait(false);

        using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        ODataResponse<JobListing>? jobsResponse = JsonSerializer.Deserialize<ODataResponse<JobListing>>(responseContent, DefaultSerializerOptions);

        return jobsResponse?.Value ?? [];
    }

    private async Task AddAuthorizationAsync(
        HttpRequestMessage request,
        HrmServerOptions options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Authentication.Mode.Equals("ApiKey", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(options.Authentication.ApiKey))
            {
                throw new InvalidOperationException("HrmServer Authentication ApiKey is required when Mode is ApiKey.");
            }

            request.Headers.Add(options.Authentication.ApiKeyHeaderName, options.Authentication.ApiKey);
            return;
        }

        if (options.Authentication.Mode.Equals("OAuth", StringComparison.OrdinalIgnoreCase))
        {
            string accessToken = await RequestAccessTokenAsync(options.Authentication.OAuth, cancellationToken)
                .ConfigureAwait(false);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return;
        }

        throw new InvalidOperationException($"Unsupported HrmServer Authentication Mode '{options.Authentication.Mode}' check your environment variable configuration.");
    }

    private static Uri BuildUrl(HrmServerOptions options, string path)
    {
        ArgumentNullException.ThrowIfNull(options);

        return string.IsNullOrWhiteSpace(options.Host)
            ? throw new InvalidOperationException("HrmServer Host must be configured.")
            : !Uri.TryCreate(options.Host, UriKind.Absolute, out Uri? baseUri)
            ? throw new InvalidOperationException("HrmServer Host must be an absolute URI.")
            : new Uri(baseUri, path);
    }

    private async Task<string> RequestAccessTokenAsync(
        HrmServerOAuthClientCredentialsOptions oauthOptions,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(oauthOptions);

        if (string.IsNullOrWhiteSpace(oauthOptions.TenantId))
        {
            throw new InvalidOperationException("HrmServer OAuth TenantId is required when Mode is OAuth.");
        }

        if (string.IsNullOrWhiteSpace(oauthOptions.ClientId))
        {
            throw new InvalidOperationException("HrmServer OAuth ClientId is required when Mode is OAuth.");
        }

        if (string.IsNullOrWhiteSpace(oauthOptions.ClientSecret))
        {
            throw new InvalidOperationException("HrmServer OAuth ClientSecret is required when Mode is OAuth.");
        }

        using HttpRequestMessage tokenRequest = new(HttpMethod.Post, $"https://login.microsoftonline.com/{oauthOptions.TenantId}/oauth2/v2.0/token")
        {
            Content = new FormUrlEncodedContent(BuildTokenForm(oauthOptions))
        };

        using HttpResponseMessage tokenResponse = await httpClient.SendAsync(tokenRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        _ = tokenResponse.EnsureSuccessStatusCode();

        OAuthTokenResponse? tokenPayload = await tokenResponse.Content.ReadFromJsonAsync<OAuthTokenResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return string.IsNullOrWhiteSpace(tokenPayload?.AccessToken)
            ? throw new InvalidOperationException("OAuth token response did not contain access_token.")
            : tokenPayload.AccessToken;
    }

    private static IEnumerable<KeyValuePair<string, string>> BuildTokenForm(HrmServerOAuthClientCredentialsOptions oauthOptions)
    {
        // OAuth client credentials flow requires tenant-issued scope and client credentials in form-url-encoded format.
        return
        [
            new("grant_type", "client_credentials"),
            new("client_id", oauthOptions.ClientId ?? string.Empty),
            new("client_secret", oauthOptions.ClientSecret ?? string.Empty),
            new("scope", OAuthScope)
        ];
    }

    /// <summary>
    /// Submits an external application as JSON to the upstream HRM server. The ExternalApplication model
    /// can include Base64-encoded resume content in the Base64Content property.
    /// </summary>
    public async Task<bool> SubmitExternalApplicationJsonAsync(ExternalApplication application, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        HrmServerOptions options = runtimeConfiguration.CurrentOptions;
        Uri requestUri = BuildUrl(options, "/api/data/RecruitmentProjects/ExternalApply");

        var payload = new
        {
            ExternalApplication = application
        };

        using HttpRequestMessage request = new(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload, options: DefaultSerializerOptions)
        };

        await AddAuthorizationAsync(request, options, cancellationToken).ConfigureAwait(false);

        HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    private sealed class OAuthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }
    }

    private sealed class ODataResponse<T>
    {
        [JsonPropertyName("@odata.count")]
        public int? Count { get; init; }

        [JsonPropertyName("value")]
        public List<T>? Value { get; init; }
    }

    private sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateOnlyFormat = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return string.IsNullOrWhiteSpace(value)
                ? default
                : DateOnly.TryParseExact(value, DateOnlyFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly)
                ? dateOnly
                : DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset dateTimeOffset)
                ? DateOnly.FromDateTime(dateTimeOffset.UtcDateTime)
                : throw new JsonException($"Unable to convert \"{value}\" to DateOnly.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateOnlyFormat, CultureInfo.InvariantCulture));
        }
    }

    private sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        private const string DateOnlyFormat = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string? value = reader.GetString();
            return string.IsNullOrWhiteSpace(value)
                ? null
                : DateOnly.TryParseExact(value, DateOnlyFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly)
                ? dateOnly
                : DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset dateTimeOffset)
                ? DateOnly.FromDateTime(dateTimeOffset.UtcDateTime)
                : throw new JsonException($"Unable to convert \"{value}\" to DateOnly.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.Value.ToString(DateOnlyFormat, CultureInfo.InvariantCulture));
        }
    }
}
