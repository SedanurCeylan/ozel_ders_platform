namespace OzelDersYonetim.Services.Notifications;
public interface INotificationService { Task CreateAsync(IEnumerable<string> userIds, string title, string message, string type, int? relatedEntityId, string? targetUrl); }
