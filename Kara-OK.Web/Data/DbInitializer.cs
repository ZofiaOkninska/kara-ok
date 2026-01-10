using Kara_OK.Web.Models.Entities;
using Kara_OK.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kara_OK.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedRoomsAsync(db);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Owner", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        const string ownerPassword = "Owner123!";
        const string customerPassword = "Customer123!";

        var seedAccounts = new[]
        {
            new SeedAccount("Owner", "owner.jane@test.com", "Jane", "Clark", ownerPassword),
            new SeedAccount("Owner", "owner.mike@test.com", "Mike", "Wazowski", ownerPassword),

            new SeedAccount("Customer", "customer.anne@test.com", "Anne", "Bush", customerPassword),
            new SeedAccount("Customer", "customer.jake@test.com", "Jake", "Clinton", customerPassword),
        };

        foreach (var acc in seedAccounts)
            await EnsureUserInRoleAsync(userManager, acc);
    }

    static async Task EnsureUserInRoleAsync(UserManager<ApplicationUser> userManager, SeedAccount acc)
    {
        var user = await userManager.FindByEmailAsync(acc.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = acc.Email,
                Email = acc.Email,
                FirstName = acc.FirstName,
                LastName = acc.LastName
            };

            var createResult = await userManager.CreateAsync(user, acc.Password);
            if (!createResult.Succeeded)
                throw new Exception($"Seed failed for {acc.Email}: " +
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, acc.Role))
        {                
            var roleResult = await userManager.AddToRoleAsync(user, acc.Role);
            if (!roleResult.Succeeded)
                throw new Exception($"AddToRole failed for {acc.Email}: " +
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedRoomsAsync(AppDbContext db)
    {
        var rooms = new[]
        {
            new SeedRoom("Pop", 20, 200m,  "freezer, 20x wireless mics, disco lights, basic mixer, basic speakers, TJ media karaoke system, projector, 6x pilots"),
            new SeedRoom("Rock", 6, 40m, "6x wireless mics, bright LEDs, punchy speakers, TJ media karaoke system, TV 65in, 2x pilots"),
            new SeedRoom("Hip-Hop", 4, 30m, "5x wireless mics, disco lights, basic speakers, TJ media karaoke system, TV 50in, 2x pilots"),
            new SeedRoom("K-Pop", 10, 100m, "freezer, 12x wireless mics, bright LEDs, basic speakers, TJ media karaoke system, projector, 4x pilots"),
            new SeedRoom("Disco", 15, 150m, "freezer, 20x wireless mics, disco lights, mixer, basic speakers, TJ media karaoke system, projector, 6x pilots")
        };

        foreach (var r in rooms)
        {
            // Idempotent seed
            var exists = await db.Rooms.AnyAsync(x => x.Name == r.Name);
            if (exists) continue;

            db.Rooms.Add(new Room
            {
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerHour = r.PricePerHour,
                IsActive = true,
                EquipmentDescription = r.EquipmentDescription
            });
        }

        await db.SaveChangesAsync();
    }

    private record SeedAccount(string Role, string Email, string FirstName, string LastName, string Password);
    private record SeedRoom(string Name, int Capacity, decimal PricePerHour, string EquipmentDescription);
}