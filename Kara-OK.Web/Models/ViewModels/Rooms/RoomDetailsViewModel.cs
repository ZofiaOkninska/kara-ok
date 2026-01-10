namespace Kara_OK.Web.Models.ViewModels.Rooms;

public sealed class RoomDetailsViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Capacity { get; init; }
    public decimal PricePerHour { get; init; }
    public string EquipmentDescription { get; init; } = "";

    public IReadOnlyList<string> EquipmentList =>
    string.IsNullOrWhiteSpace(EquipmentDescription)
        ? Array.Empty<string>()
        : EquipmentDescription
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .ToList();
}