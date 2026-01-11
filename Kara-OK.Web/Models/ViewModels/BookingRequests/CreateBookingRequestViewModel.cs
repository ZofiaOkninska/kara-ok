using System.ComponentModel.DataAnnotations;
using Kara_OK.Web.Models.Validation;

namespace Kara_OK.Web.Models.ViewModels.BookingRequests;

public sealed class CreateBookingRequestViewModel
{
    public int RoomId { get; init; }

    // Only for display purposes
    public string RoomName { get; init; } = string.Empty;
    public decimal CurrentPricePerHour { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    [Range(ValidationConstants.BookingMinHours, ValidationConstants.BookingMaxHours)]
    public int DurationHours { get; set; }    
    [Range(ValidationConstants.BookingMinDurationExtraMinutes, ValidationConstants.BookingMaxDurationExtraMinutes)]
    public int DurationExtraMinutes { get; set; } 

    public int DurationTotalMinutes => (DurationHours * 60) + DurationExtraMinutes;

    [Range(ValidationConstants.BookingMinPeopleCount, ValidationConstants.BookingMaxPeopleCount)]
    public int PeopleCount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 1) Extra minutes in 15-minute steps
        if (DurationExtraMinutes % 15 != 0)
            yield return new ValidationResult(
                "Minutes must be 0, 15, 30 or 45.",
                new[] { nameof(DurationExtraMinutes) });

        // 2) Total duration 15-360
        if (DurationTotalMinutes < ValidationConstants.BookingMinDurationMinutes ||
            DurationTotalMinutes > ValidationConstants.BookingMaxDurationMinutes)
            yield return new ValidationResult(
                $"Duration must be between {ValidationConstants.BookingMinDurationMinutes} and {ValidationConstants.BookingMaxDurationMinutes} minutes.",
                new[] { nameof(DurationHours), nameof(DurationExtraMinutes) });

        // 3) Total duration in 15-minute steps (this also catches e.g. 1h10)
        if (DurationTotalMinutes % 15 != 0)
            yield return new ValidationResult(
                "Total duration must be in 15-minute steps.",
                new[] { nameof(DurationHours), nameof(DurationExtraMinutes) });
    }
}