using Kara_OK.Web.Data;
using Kara_OK.Web.Helpers;
using Kara_OK.Web.Models.Identity;
using Kara_OK.Web.Models.ViewModels.BookingRequests;
using Kara_OK.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kara_OK.Web.Controllers;

[Authorize(Roles = "Owner,Customer")]
public class BookingRequestsController : Controller
{
    private readonly AppDbContext _db;
    private readonly BookingRequestsCommandService _command;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingRequestsController(AppDbContext db, BookingRequestsCommandService command, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _command = command;
        _userManager = userManager;
    }

    // GET: /BookingRequests
    public async Task<IActionResult> Index()
    {
        var isOwner = User.IsInRole("Owner");
        var userId = _userManager.GetUserId(User);

        var query = _db.BookingRequests
            .AsNoTracking()
            .Include(br => br.Room)
            .AsQueryable();

        if (!isOwner)
        {
            // Customer only relevant requests
            query = query.Where(br => br.CustomerUserId == userId);
        }

        // Owner can see customer details
        if (isOwner)
        {
            query = query.Include(br => br.CustomerUser);
        }

        var items = await query
            .OrderByDescending(br => br.StartDateTime)
            .Select(br => new BookingRequestListViewModel
            {
                Id = br.Id,
                RoomId = br.RoomId,
                RoomName = br.Room.Name,
                StartDateTime = br.StartDateTime,
                DurationMinutes = br.DurationMinutes,
                PeopleCount = br.PeopleCount,
                Status = br.Status.ToString(),
                PricePerHourAtRequest = br.PricePerHourAtRequest,
                TotalPrice = br.TotalPrice,
                CustomerName = isOwner ? (br.CustomerUser.FirstName + " " + br.CustomerUser.LastName) : null
            })
            .ToListAsync();

        ViewBag.Title = isOwner ? "All requests" : "My requests";
        return View(items);
    }

    // GET: /BookingRequests/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var isOwner = User.IsInRole("Owner");
        var userId = _userManager.GetUserId(User);

        var query = _db.BookingRequests
            .AsNoTracking()
            .Include(br => br.Room)
            .AsQueryable();

        if (isOwner)
        {
            query = query
                .Include(br => br.CustomerUser)
                .Include(br => br.DecidedByUser);
        }
        else
        {
            // Customer only relevant requests
            query = query.Where(br => br.CustomerUserId == userId);
        }

        var vm = await query
            .Where(br => br.Id == id)
            .Select(br => new BookingRequestDetailsViewModel
            {
                Id = br.Id,
                RoomId = br.RoomId,
                RoomName = br.Room.Name,
                StartDateTime = br.StartDateTime,
                DurationMinutes = br.DurationMinutes,
                PeopleCount = br.PeopleCount,
                Status = br.Status.ToString(),
                PricePerHourAtRequest = br.PricePerHourAtRequest,
                TotalPrice = br.TotalPrice,
                IsPaid = br.IsPaid,
                DecidedAt = br.DecidedAt,
                CustomerName = isOwner ? (br.CustomerUser.FirstName + " " + br.CustomerUser.LastName) : null,
                DecidedByName = isOwner && br.DecidedByUser != null
                    ? (br.DecidedByUser.FirstName + " " + br.DecidedByUser.LastName)
                    : null //TODO: if needed add decider email
            })
            .SingleOrDefaultAsync();

        if (vm is null) return NotFound();
        return View(vm);
    }

    // GET: /BookingRequests/Create?roomId=1
    [HttpGet]
    [Authorize(Roles = "Customer")]
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
    [Authorize(Roles = "Customer")]
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