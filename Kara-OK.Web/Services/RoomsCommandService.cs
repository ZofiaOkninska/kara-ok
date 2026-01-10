using Kara_OK.Web.Data;
using Kara_OK.Web.Models.Entities;
using Kara_OK.Web.Models.Validation;
using Kara_OK.Web.Models.ViewModels.Rooms;
using Microsoft.EntityFrameworkCore;

namespace Kara_OK.Web.Services;

public sealed class RoomsCommandService
{
    private readonly AppDbContext _db;

    public RoomsCommandService(AppDbContext db)
    {
        _db = db;
    }

    public Task<EditRoomViewModel?> GetEditModelAsync(int id)
        => _db.Rooms
            .Where(r => r.IsActive && r.Id == id)
            .Select(r => new EditRoomViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerHour = r.PricePerHour,
                EquipmentDescription = r.EquipmentDescription
            })
            .SingleOrDefaultAsync();

    public async Task<(bool Success, string? Error)> UpdateAsync(EditRoomViewModel model)
    {
        // Validations
        var name = model.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name is required.");

        if (model.PricePerHour > ValidationConstants.RoomMaxPricePerHour)
            return (false, $"Price per hour cannot exceed {ValidationConstants.RoomMaxPricePerHour}.");


        var room = await _db.Rooms.SingleOrDefaultAsync(r => r.IsActive && r.Id == model.Id);
        if (room is null) 
            return (false, "Room not found.");

        room.Name = name;
        room.Capacity = model.Capacity;
        room.PricePerHour = model.PricePerHour;

        // Normalize equipment: comma-separated, trim item, remove empties
        var normalizedEquipment = string.Join(", ",
            (model.EquipmentDescription ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => e.Length > 0)
        );

        room.EquipmentDescription = normalizedEquipment;

        await _db.SaveChangesAsync();
        return (true, null);
    }
}