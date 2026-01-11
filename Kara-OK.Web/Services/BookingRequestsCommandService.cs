using Kara_OK.Web.Data;
using Kara_OK.Web.Helpers;
using Kara_OK.Web.Models.Entities;
using Kara_OK.Web.Models.Enums;
using Kara_OK.Web.Models.Identity;
using Kara_OK.Web.Models.ViewModels.BookingRequests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kara_OK.Web.Services;

public sealed class BookingRequestsCommandService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingRequestsCommandService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<(bool Success, List<(string Field, string Message)> Errors, int? CreatedId)> CreateAsync(
        CreateBookingRequestViewModel model,
        ClaimsPrincipal user)
    {
        var errors = new List<(string, string)>();

        // Room must exist + active
        var room = await _db.Rooms.FindAsync(model.RoomId);
        if (room is null || !room.IsActive)
        {
            errors.Add((string.Empty, "Selected room does not exist."));
            return (false, errors, null);
        }

        // Customer id from auth (NOT from UI)
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            errors.Add((string.Empty, "You must be logged in."));
            return (false, errors, null);
        }

        // StartDateTime required + buffer + 15-minute grid
        if (model.StartDateTime == default)
        {
            errors.Add((nameof(model.StartDateTime), "Start time is required."));
        }
        else
        {
            var now = DateTime.Now;
            var minAllowed = TimeHelpers.CeilToQuarterHour(now.AddMinutes(15));

            if (model.StartDateTime < minAllowed)
            {
                errors.Add((nameof(model.StartDateTime),
                    $"Choose a time from {minAllowed:yyyy-MM-dd HH:mm} or later."));
            }

            if (!TimeHelpers.IsQuarterHour(model.StartDateTime))
            {
                errors.Add((nameof(model.StartDateTime),
                    "Start time must be in 15-minute steps (e.g., 13:15, 13:30, 13:45)."));
            }
        }

        // 15-minute steps
        var durationMinutes = model.DurationTotalMinutes;

        if (durationMinutes < 15 || durationMinutes > 360)
            errors.Add((nameof(model.DurationTotalMinutes), "Duration must be between 15 minutes and 6 hours."));

        if (durationMinutes % 15 != 0)
            errors.Add((nameof(model.DurationTotalMinutes), "Duration must be in 15-minute steps (15, 30, 45, ...)."));

        // PeopleCount vs room capacity
        if (model.PeopleCount > room.Capacity)
        {
            errors.Add((nameof(model.PeopleCount), $"Maximum number of people for this room is {room.Capacity}."));
        }

        if (errors.Count > 0)
            return (false, errors, null);

        // Snapshot price + total
        var pricePerHour = room.PricePerHour;
        var totalPrice = pricePerHour * (durationMinutes / 60m);

        var request = new BookingRequest
        {
            RoomId = room.Id,
            CustomerUserId = userId,
            StartDateTime = model.StartDateTime,
            DurationMinutes = durationMinutes,
            PeopleCount = model.PeopleCount,
            Status = BookingStatus.Pending,
            PricePerHourAtRequest = pricePerHour,
            TotalPrice = totalPrice,
            IsPaid = false
        };

        _db.BookingRequests.Add(request);
        await _db.SaveChangesAsync();

        return (true, new(), request.Id);
    }

    public async Task<(bool Success, List<(string Field, string Message)> Errors)>
    AcceptAsync(int requestId, ClaimsPrincipal owner)
    {
        var errors = new List<(string, string)>();

        var ownerId = _userManager.GetUserId(owner);
        if (string.IsNullOrWhiteSpace(ownerId))
        {
            errors.Add((string.Empty, "You must be logged in."));
            return (false, errors);
        }

        var req = await _db.BookingRequests.SingleOrDefaultAsync(r => r.Id == requestId);
        if (req is null)
        {
            errors.Add((string.Empty, "Request not found."));
            return (false, errors);
        }

        if (req.Status != BookingStatus.Pending)
        {
            errors.Add((string.Empty, "Only Pending requests can be accepted/rejected."));
            return (false, errors);
        }

        req.Status = BookingStatus.Confirmed;
        req.DecidedAt = DateTime.Now;
        req.DecidedByUserId = ownerId;

        await _db.SaveChangesAsync();
        return (true, errors);
    }

    public async Task<(bool Success, List<(string Field, string Message)> Errors)>
        RejectAsync(int requestId, ClaimsPrincipal owner)
    {
        var errors = new List<(string, string)>();

        var ownerId = _userManager.GetUserId(owner);
        if (string.IsNullOrWhiteSpace(ownerId))
        {
            errors.Add((string.Empty, "You must be logged in."));
            return (false, errors);
        }

        var req = await _db.BookingRequests.SingleOrDefaultAsync(r => r.Id == requestId);
        if (req is null)
        {
            errors.Add((string.Empty, "Request not found."));
            return (false, errors);
        }

        if (req.Status != BookingStatus.Pending)
        {
            errors.Add((string.Empty, "Only Pending requests can be accepted/rejected."));
            return (false, errors);
        }

        req.Status = BookingStatus.Rejected;
        req.DecidedAt = DateTime.Now;
        req.DecidedByUserId = ownerId;

        await _db.SaveChangesAsync();
        return (true, errors);
    }

}