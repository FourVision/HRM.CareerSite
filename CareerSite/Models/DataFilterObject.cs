namespace Hrm.CareerSite.Models;

/// <summary>
/// Represents the filter payload sent to the upstream HRM job openings endpoint.
/// </summary>
public sealed class DataFilterObject
{
    /// <summary>
    /// Gets or sets the selected tab identifier in the HRM search UI.
    /// </summary>
    public int TabId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether closing-soon jobs should be prioritized.
    /// </summary>
    public bool EnableClosingSoon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only applied jobs should be returned.
    /// </summary>
    public bool IsApplied { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether recommended jobs should be included.
    /// </summary>
    /// <remarks>
    /// The property name mirrors the upstream API contract.
    /// </remarks>
    public bool EnableRecomended { get; set; }

    /// <summary>
    /// Gets or sets the free-text search input.
    /// </summary>
    public required string SearchText { get; set; }

    /// <summary>
    /// Gets or sets department identifiers to filter by.
    /// </summary>
    public required IEnumerable<Guid> DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets job identifiers marked as favorite by the current user.
    /// </summary>
    public required List<Guid> FavoriteJobIds { get; set; }

    /// <summary>
    /// Gets or sets the requested full-time-equivalent filter value.
    /// </summary>
    public decimal FteValue { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of days until a job opening date.
    /// </summary>
    public int OpenInDays { get; set; }

    /// <summary>
    /// Gets or sets the sort field used by the upstream endpoint.
    /// </summary>
    public string SortOn { get; set; } = "Relevance";

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int Top { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of items to skip.
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Gets or sets the preferred language used when requesting localized values.
    /// </summary>
    public required string Language { get; set; }
}
