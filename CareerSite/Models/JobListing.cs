namespace Hrm.CareerSite.Models;

/// <summary>
/// Represents a job listing shown in the CareerSite UI.
/// </summary>
/// <param name="Id">The unique job listing identifier.</param>
/// <param name="ApplicationId">The identifier used to apply for the job.</param>
/// <param name="JobTitle">The role title displayed to end users.</param>
/// <param name="DepartmentName">The department associated with the role.</param>
/// <param name="FTE">The full-time-equivalent value of the role, if available.</param>
/// <param name="Type">The employment type label.</param>
/// <param name="Description">A short job summary.</param>
/// <param name="DaysToClose">The number of days remaining before closing.</param>
/// <param name="IsApplied">Indicates whether the current user has already applied.</param>
/// <param name="OpenDate">The date the vacancy became available.</param>
/// <param name="JobAdText">The full advertisement text for details view scenarios.</param>
public sealed record JobListing(
    Guid Id,
    Guid ApplicationId,
    string JobTitle,
    string DepartmentName,
    decimal? FTE,
    string Type,
    string Description,
    int? DaysToClose,
    bool IsApplied,
    DateOnly OpenDate,
    string JobAdText);
