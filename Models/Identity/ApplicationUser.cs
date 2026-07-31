using Microsoft.AspNetCore.Identity;
using OzelDersYonetim.Models.Students;
using OzelDersYonetim.Models.Notifications;

namespace OzelDersYonetim.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsActive { get; set; } = true;

    public bool MustChangePassword { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public StudentProfile? StudentProfile { get; set; }
    public ICollection<UserNotification> Notifications { get; set; } = new List<UserNotification>();
}
