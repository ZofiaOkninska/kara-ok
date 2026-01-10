namespace Kara_OK.Web.Models.ViewModels.Rooms;

public sealed class RoomListViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Capacity { get; init; }
    public decimal PricePerHour { get; init; }
}