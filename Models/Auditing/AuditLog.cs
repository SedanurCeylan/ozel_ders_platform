using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.Auditing;
public class AuditLog
{
    public int Id { get; set; }
    [StringLength(450)] public string? ApplicationUserId { get; set; }
    [Required,StringLength(100)] public string ActionType { get; set; }=string.Empty;
    [Required,StringLength(100)] public string EntityType { get; set; }=string.Empty;
    public int? EntityId { get; set; }
    [Required,StringLength(2000)] public string Description { get; set; }=string.Empty;
    [StringLength(80)] public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
}
