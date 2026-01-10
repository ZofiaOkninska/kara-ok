using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Kara_OK.Web.Models;
using Kara_OK.Web.Models.Identity;
using Kara_OK.Web.Services;

namespace Kara_OK.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoomsQueryService _rooms;
    
    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoomsQueryService rooms)
    {
        _logger = logger;
        _userManager = userManager;
        _rooms = rooms;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.FirstName = user?.FirstName;
        }

        var rooms = await _rooms.GetActiveRoomsAsync();
        return View(rooms);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
