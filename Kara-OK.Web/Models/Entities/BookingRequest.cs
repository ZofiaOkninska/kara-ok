using Kara_OK.Web.Models.Enums;
using Kara_OK.Web.Models.Identity;
using Kara_OK.Web.Models.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kara_OK.Web.Models.Entities;

public class BookingRequest : IValidatableObject
{
    public int Id { get; set; }

    // Room foreign key
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    // Who made request: ApplicationUser (customer) foreign key
    [Required]
    public string CustomerUserId { get; set; } = null!;
    public ApplicationUser CustomerUser { get; set; } = null!;

    // When and how long
    public DateTime StartDateTime { get; set; }
    
    [Range(ValidationConstants.BookingMinDurationMinutes, ValidationConstants.BookingMaxDurationMinutes)]
    public int DurationMinutes { get; set; } 
    
    [Range(ValidationConstants.BookingMinPeopleCount, ValidationConstants.BookingMaxPeopleCount)]
    public int PeopleCount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    // Snapshot of price at the time of request
    public decimal PricePerHourAtRequest { get; set; }
    
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal TotalPrice { get; set; }

    // Payment info
    public bool IsPaid { get; set; }

    // Owner decision info
    public DateTime? DecidedAt { get; set; }
    public string? DecidedByUserId { get; set; }
    public ApplicationUser? DecidedByUser { get; set; }

    // Computed (not in database)
    [NotMapped]
    public DateTime EndDateTime => StartDateTime.AddMinutes(DurationMinutes);

    // Validations
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Duration time validation: must be in 15-minute increments
        if (DurationMinutes % 15 != 0)
            yield return new ValidationResult(
                "Duration must be in 15-minute increments (15, 30, 45, ...).",
                new[] { nameof(DurationMinutes) });

        // Cohesion of owner decision fields with status
        var isDecided = Status == BookingStatus.Confirmed || Status == BookingStatus.Rejected;

        if (!isDecided)
        {   
            if (DecidedAt != null || DecidedByUserId != null)
            {
                yield return new ValidationResult(
                    "For the Pending/Cancelled statuses, the owner decision fields (DecidedAt and DecidedByUserId) must be empty.",
                    new[] { nameof(DecidedAt), nameof(DecidedByUserId) });
            }
        }
        else
        {
            if (DecidedAt == null || string.IsNullOrWhiteSpace(DecidedByUserId))
            {
                yield return new ValidationResult(
                    "For the Confirmed/Rejected statuses, the owner decision fields (DecidedAt and DecidedByUserId) must be set.",
                    new[] { nameof(DecidedAt), nameof(DecidedByUserId) });
            }
        }
    }
}