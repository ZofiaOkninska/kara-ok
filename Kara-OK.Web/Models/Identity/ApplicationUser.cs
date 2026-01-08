using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Kara_OK.Web.Models.Identity;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
}