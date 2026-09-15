using Hrm.CareerSite;
using Hrm.CareerSite.Configuration;
using Hrm.CareerSite.Models;
using Hrm.CareerSite.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddOptions<HrmServerOptions>()
    .Bind(builder.Configuration.GetSection(HrmServerOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<HrmServerRuntimeConfiguration>();
builder.Services.AddHttpClient<HrmServerClient>();
builder.Services.AddScoped<JobListingsApiClient>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/jobs", async (
    HttpRequest request,
    HrmServerClient hrmServerClient,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    ArgumentNullException.ThrowIfNull(request);

    try
    {
        DataFilterObject filterObject = BuildFilterObject(request);
        IReadOnlyList<JobListing> jobs = await hrmServerClient.GetJobsAsync(filterObject, cancellationToken).ConfigureAwait(false);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Returning {JobCount} job listings from HRM server.", jobs.Count);
        }
        return Results.Ok(jobs);
    }
    catch (HttpRequestException exception)
    {
        logger.LogError(exception, "Failed to fetch jobs from HRM server.");
        return Results.Problem("Could not reach HRM server jobs function.", statusCode: StatusCodes.Status502BadGateway);
    }
    catch (InvalidOperationException exception)
    {
        logger.LogError(exception, "Invalid HRM server configuration.");
        return Results.Problem("HRM server configuration is invalid.", statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static DataFilterObject BuildFilterObject(HttpRequest request)
{
    return new DataFilterObject
    {
        Language = string.IsNullOrWhiteSpace(request.Headers.AcceptLanguage.ToString())
            ? "en"
            : request.Headers.AcceptLanguage.ToString(),
        Top = ParsePositiveInt(request.Query["top"].ToString(), 10),
        Skip = ParseNonNegativeInt(request.Query["skip"].ToString(), 0),
        SearchText = request.Query["searchText"].ToString(),
        DepartmentId = [],
        FavoriteJobIds = []
    };
}

static int ParsePositiveInt(string value, int fallback)
{
    return int.TryParse(value, out int parsedValue) && parsedValue > 0
        ? parsedValue
        : fallback;
}

static int ParseNonNegativeInt(string value, int fallback)
{
    return int.TryParse(value, out int parsedValue) && parsedValue >= 0
        ? parsedValue
        : fallback;
}
