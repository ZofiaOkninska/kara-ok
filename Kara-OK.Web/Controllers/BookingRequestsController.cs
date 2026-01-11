using Kara_OK.Web.Data;
using Kara_OK.Web.Helpers;
using Kara_OK.Web.Models.ViewModels.BookingRequests;
using Kara_OK.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kara_OK.Web.Controllers;

[Authorize(Roles = "Customer")]
public class BookingRequestsController : Controller
{
    private readonly AppDbContext _db;
    private readonly BookingRequestsCommandService _command;

    public BookingRequestsController(AppDbContext db, BookingRequestsCommandService command)
    {
        _db = db;
        _command = command;
    }

    // GET: /BookingRequests/Create?roomId=1
    [HttpGet]
    public async Task<IActionResult> Create(int roomId)
    {
        var room = await _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsActive && r.Id == roomId)
            .Select(r => new { r.Id, r.Name, r.PricePerHour })
            .SingleOrDefaultAsync();

        if (room is null) return NotFound();

        var defaultStart = TimeHelpers.CeilToQuarterHour(DateTime.Now.AddMinutes(15));

        var roomViewModel = new CreateBookingRequestViewModel
        {
            RoomId = room.Id,
            RoomName = room.Name, 
            CurrentPricePerHour = room.PricePerHour,
            StartDateTime = defaultStart, 
            DurationHours = 1,
            DurationExtraMinutes = 0,
            PeopleCount = 2
        };

        return View(roomViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingRequestViewModel model, string? action)
    {
        if (!ModelState.IsValid)
            return View(model);

        var pricePerHour = await _db.Rooms
            .Where(r => r.IsActive && r.Id == model.RoomId)
            .Select(r => r.PricePerHour)
            .SingleOrDefaultAsync();

        model.CurrentPricePerHour = pricePerHour;

        if (action == "calc")
        {
            // Re-display the form with the calculated price
            ViewBag.EstimatedTotal =
                (model.CurrentPricePerHour * model.DurationTotalMinutes / 60m);
            return View(model);
        }

        var (success, error, createdId) = await _command.CreateAsync(model, User);
        if (!success)
        {
            foreach (var (field, message) in error)
                ModelState.AddModelError(field, message);
            return View(model);
        }

        // TODO: in the furure redirect to /Requests 
        return RedirectToAction("Details", "Rooms", new { id = model.RoomId });
    }
}