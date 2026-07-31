using System.Security.Claims;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Auditing;
namespace OzelDersYonetim.Services.Auditing;
public class AuditService(ApplicationDbContext dbContext,IHttpContextAccessor contextAccessor):IAuditService
{
    public async Task LogAsync(string actionType,string entityType,int? entityId,string description)
    {
        var context=contextAccessor.HttpContext;dbContext.AuditLogs.Add(new AuditLog{ApplicationUserId=context?.User.FindFirstValue(ClaimTypes.NameIdentifier),ActionType=actionType,EntityType=entityType,EntityId=entityId,Description=description,IpAddress=context?.Connection.RemoteIpAddress?.ToString()});await dbContext.SaveChangesAsync();
    }
}
