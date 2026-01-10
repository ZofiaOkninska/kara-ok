using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kara_OK.Web.Models.Entities;
using Kara_OK.Web.Models.Identity;

namespace Kara_OK.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
}