using Hrm.CareerSite.Models;

namespace Hrm.CareerSite.Services;

/// <summary>
/// Encapsulates access to job listings for UI components.
/// </summary>
public sealed class JobListingsApiClient(HrmServerClient hrmServerClient)
{
    /// <summary>
    /// Gets available job listings from the backing HRM service integration.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the HTTP call.</param>
    /// <returns>A read-only collection of job listings.</returns>
    public async Task<IReadOnlyList<JobListing>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        DataFilterObject filterObject = new()
        {
            Language = "en",
            Top = 10,
            Skip = 0,
            SearchText = string.Empty,
            DepartmentId = [],
            FavoriteJobIds = []
        };

        return await hrmServerClient.GetJobsAsync(filterObject, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Submits an application for the specified job as JSON. The application model may contain a Base64-encoded file in ExternalApplication.Base64Content.
    /// Returns true when the submission succeeded, false when it failed (for example when the server returned a non-success status code).
    /// </summary>
    public async Task<bool> SubmitApplicationAsync(ExternalApplication application, CancellationToken cancellationToken = default)
    {
        return await hrmServerClient.SubmitExternalApplicationJsonAsync(application, cancellationToken).ConfigureAwait(false);
    }
}
