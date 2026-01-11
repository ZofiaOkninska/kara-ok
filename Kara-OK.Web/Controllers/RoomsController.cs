using Kara_OK.Web.Models.ViewModels.Rooms;
using Kara_OK.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kara_OK.Web.Controllers;

public class RoomsController : Controller
{
    private readonly RoomsQueryService _query;
    private readonly RoomsCommandService _command;

    public RoomsController(RoomsQueryService query, RoomsCommandService command)
    {
        _query = query;
        _command = command;
    }

    // GET: /Rooms
    public async Task<IActionResult> Index()
    {
        var list = await _query.GetActiveRoomsAsync();
        return View(list);
    }

    // GET: /Rooms/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var room = await _query.GetActiveRoomDetailsAsync(id);
        if (room is null) return NotFound();
        return View(room);
    }

    // GET: /Rooms/Edit/{id}
    [Authorize(Roles = "Owner")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _command.GetEditModelAsync(id);
        if (room is null) return NotFound();
        return View(room);
    }

    [Authorize(Roles = "Owner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditRoomViewModel model)
    {
        // Validation for equipment comma-separated list
        if (!string.IsNullOrWhiteSpace(model.EquipmentDescription))
        {
            // split by comma, keep empty entries to detect ", ,"
            var rawParts = model.EquipmentDescription.Split(',');

            // 1) forbid empty items (",," or ", ,")
            if (rawParts.Any(p => string.IsNullOrWhiteSpace(p)))
            {
                ModelState.AddModelError(
                    nameof(model.EquipmentDescription),
                    "Equipment must be comma-separated. Don't leave empty items (e.g., use \"Mic, Speakers\" not \"Mic, , Speakers\")."
                );
            }

            // 2) forbid semicolons
            if (model.EquipmentDescription.Contains(';'))
            {
                ModelState.AddModelError(
                    nameof(model.EquipmentDescription),
                    "Use commas (,) to separate items, not semicolons (;)."
                );
            }
        }
        
        if (!ModelState.IsValid) return View(model);

        var (success, error) = await _command.UpdateAsync(model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Update failed.");
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}