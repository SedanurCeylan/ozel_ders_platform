namespace OzelDersYonetim.Services.Notifications;
public interface IEmailTemplateService { string Build(string title, string message, DateTime? relevantDate = null, string? buttonUrl = null, string buttonText = "Sisteme Giriş Yap"); }
