using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Notifications;
namespace OzelDersYonetim.Services.Notifications;
public class EmailService(IOptions<EmailSettings> options, ApplicationDbContext dbContext, ILogger<EmailService> logger) : IEmailService
{
    public async Task<bool> SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlBody, string emailType, int? relatedEntityId = null, string? logBody = null)
    {
        var log = new EmailLog { RecipientEmail = recipientEmail, RecipientName = recipientName, Subject = subject, Body = logBody ?? htmlBody, EmailType = emailType, RelatedEntityId = relatedEntityId };
        try
        {
            var s = options.Value; if (string.IsNullOrWhiteSpace(s.SmtpServer) || string.IsNullOrWhiteSpace(s.SenderEmail)) throw new InvalidOperationException("SMTP ayarları henüz yapılandırılmamış.");
            using var client = new SmtpClient(s.SmtpServer, s.SmtpPort) { EnableSsl = s.EnableSsl, Credentials = new NetworkCredential(s.Username, s.Password) };
            using var message = new MailMessage { From = new MailAddress(s.SenderEmail, s.SenderName), Subject = subject, Body = htmlBody, IsBodyHtml = true }; message.To.Add(new MailAddress(recipientEmail, recipientName)); await client.SendMailAsync(message); log.IsSuccessful = true; log.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex) { log.ErrorMessage = ex.Message; logger.LogWarning(ex, "E-posta gönderilemedi: {Recipient}", recipientEmail); }
        dbContext.EmailLogs.Add(log); await dbContext.SaveChangesAsync(); return log.IsSuccessful;
    }
}
