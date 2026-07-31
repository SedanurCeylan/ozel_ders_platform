using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Notifications;
namespace OzelDersYonetim.Services.Notifications;
public class NotificationService(ApplicationDbContext dbContext) : INotificationService
{
    public async Task CreateAsync(IEnumerable<string> userIds, string title, string message, string type, int? relatedEntityId, string? targetUrl) { foreach(var id in userIds.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct()) dbContext.UserNotifications.Add(new UserNotification { ApplicationUserId=id, Title=title, Message=message, NotificationType=type, RelatedEntityId=relatedEntityId, TargetUrl=targetUrl }); await dbContext.SaveChangesAsync(); }
}
