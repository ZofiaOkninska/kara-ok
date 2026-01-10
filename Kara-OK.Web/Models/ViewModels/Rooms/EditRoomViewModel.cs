using System.ComponentModel.DataAnnotations;
using Kara_OK.Web.Models.Validation;

namespace Kara_OK.Web.Models.ViewModels.Rooms;

public sealed class EditRoomViewModel
{
    public int Id { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(ValidationConstants.RoomNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Range(ValidationConstants.RoomMinCapacity, ValidationConstants.RoomMaxCapacity)]
    public int Capacity { get; set; }

    [Range(typeof(decimal), "0", "10000")]
    public decimal PricePerHour { get; set; }

    [MaxLength(ValidationConstants.EquipmentDescriptionMaxLength)]
    public string? EquipmentDescription { get; set; } = string.Empty;
}