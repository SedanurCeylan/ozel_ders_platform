namespace OzelDersYonetim.Services.Notifications;
public interface IEmailService { Task<bool> SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlBody, string emailType, int? relatedEntityId = null, string? logBody = null); }
