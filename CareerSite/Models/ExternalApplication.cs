namespace Hrm.CareerSite.Models;
using System.ComponentModel.DataAnnotations;
using Hrm.CareerSite.Enums;

/// <summary>
/// Gets or sets the applicant's and application data.
/// </summary>
public sealed class ExternalApplication
{
    /// <summary>
    /// Gets or sets the applicant's first name.
    /// </summary>
    [Required(ErrorMessage = "First name is required."), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's last name prefix, if applicable.
    /// </summary>
    [MaxLength(25)]
    public string LastNamePrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's last name.
    /// </summary>
    [Required(ErrorMessage = "Last name is required."), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's email address.
    /// </summary>
    [Required(ErrorMessage = "Email address is required."), MaxLength(256)]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's phone number.
    /// </summary>
    [Required(ErrorMessage = "Phone number is required."), MaxLength(256)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's country of residence.
    /// </summary>
    [Required(ErrorMessage = "Country / Location is required.")]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's LinkedIn profile URL.
    /// </summary>
    [MaxLength(2000)]
    public string LinkedInProfile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's portfolio or personal website URL.
    /// </summary>
    [MaxLength(2000)]
    public string PortfolioUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's motivation or cover letter.
    /// </summary>
    [MaxLength(1000)]
    public string Motivation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the applicant is legally allowed to work in this country.
    /// </summary>
    public bool WorkEligibility { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the applicant requires visa sponsorship.
    /// </summary>
    public bool RequiresSponsorship { get; set; }

    /// <summary>
    /// Gets or sets the earliest date the applicant is available to start.
    /// </summary>
    public DateOnly? AvailableFrom { get; set; }

    /// <summary>
    /// Gets or sets the applicant's notice period.
    /// </summary>
    [MaxLength(10)]
    public string NoticePeriod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's gender.
    /// </summary>
    public Gender Gender { get; set; } = Gender.None;

    /// <summary>
    /// Gets or sets the applicant's nationality.
    /// </summary>
    public string Nationality { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's disability status.
    /// </summary>
    [MaxLength(60)]
    public string DisabilityStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the job.
    /// </summary>
    [Required]
    public Guid JobId { get; set; }

    /// <summary>
    /// Gets or sets the name of the uploaded file.
    /// </summary>
    [MaxLength(100)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type of the uploaded file.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Base64-encoded content of the uploaded file.
    /// </summary>
    public string Base64Content { get; set; } = string.Empty;
}
