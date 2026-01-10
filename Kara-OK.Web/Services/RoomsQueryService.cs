using Kara_OK.Web.Data;
using Kara_OK.Web.Models.ViewModels.Rooms;
using Microsoft.EntityFrameworkCore;

namespace Kara_OK.Web.Services;

public sealed class RoomsQueryService
{
    private readonly AppDbContext _db;

    public RoomsQueryService(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<RoomListViewModel>> GetActiveRoomsAsync()
        => _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(r => r.Capacity)
            .Select(r => new RoomListViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerHour = r.PricePerHour
            })
            .ToListAsync();

    public Task<RoomDetailsViewModel?> GetActiveRoomDetailsAsync(int id)
        => _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsActive && r.Id == id)
            .Select(r => new RoomDetailsViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerHour = r.PricePerHour,
                EquipmentDescription = r.EquipmentDescription
            })
            .SingleOrDefaultAsync();
}