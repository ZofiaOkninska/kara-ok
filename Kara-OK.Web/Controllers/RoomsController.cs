using Kara_OK.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kara_OK.Web.Controllers;

public class RoomsController(RoomsQueryService rooms) : Controller
{
    // GET: /Rooms
    public async Task<IActionResult> Index()
    {
        var list = await rooms.GetActiveRoomsAsync();
        return View(list);
    }

    // GET: /Rooms/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var room = await rooms.GetActiveRoomDetailsAsync(id);
        if (room is null) return NotFound();
        return View(room);
    }
}