namespace Kara_OK.Web.Models.ViewModels.BookingRequests;

public sealed class BookingRequestListViewModel
{
    public int Id { get; init; }
    public int RoomId { get; init; }
    public string RoomName { get; init; } = string.Empty;

    public DateTime StartDateTime { get; init; }
    public int DurationMinutes { get; init; }
    public DateTime EndDateTime => StartDateTime.AddMinutes(DurationMinutes);

    public int PeopleCount { get; init; }
    public string Status { get; init; } = string.Empty;

    public decimal PricePerHourAtRequest { get; init; }
    public decimal TotalPrice { get; init; }

    // Only for owner view
    public string? CustomerName { get; init; }
    // TODO: add customer email and isPaid if needed
}