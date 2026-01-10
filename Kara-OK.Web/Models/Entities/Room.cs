using System.ComponentModel.DataAnnotations;
using Kara_OK.Web.Models.Validation;

namespace Kara_OK.Web.Models.Entities;

public class Room
{
    public int Id { get; set; }

    [Required(AllowEmptyStrings=false)] 
    [MaxLength(ValidationConstants.RoomNameMaxLength)]
    public string Name { get; set; } = null!;

    [Range(ValidationConstants.RoomMinCapacity, ValidationConstants.RoomMaxCapacity)]
    public int Capacity { get; set; }

    [Range(typeof(decimal), "0", "10000")]
    public decimal PricePerHour { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(AllowEmptyStrings=false)] 
    [MaxLength(ValidationConstants.EquipmentDescriptionMaxLength)]
    public string EquipmentDescription { get; set; } = "No equipment added yet";

    // Navigation
    public List<BookingRequest> BookingRequests { get; set; } = new();
}