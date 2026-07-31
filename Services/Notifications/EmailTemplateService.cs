using System.Net;
using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
namespace OzelDersYonetim.Services.Notifications;
public class EmailTemplateService(IOptions<EmailSettings> options) : IEmailTemplateService
{
    public string Build(string title, string message, DateTime? relevantDate = null, string? buttonUrl = null, string buttonText = "Sisteme Giriş Yap")
    {
        var settings = options.Value; var url = buttonUrl ?? settings.SiteUrl + "/Identity/Account/Login";
        return $"""<!doctype html><html lang="tr"><body style="margin:0;background:#f7f4ee;font-family:Arial,sans-serif;color:#14233c"><div style="max-width:620px;margin:30px auto;background:#fff;border-radius:16px;overflow:hidden"><div style="padding:24px 30px;background:#14233c;color:#fff;font-size:22px"><b>∑ Matematik Atölyesi</b></div><div style="padding:32px"><h1 style="font-size:26px">{WebUtility.HtmlEncode(title)}</h1><p style="line-height:1.7;color:#566176">{WebUtility.HtmlEncode(message)}</p>{(relevantDate.HasValue ? $"<p><b>İlgili tarih:</b> {relevantDate.Value:dd.MM.yyyy HH:mm}</p>" : "")}<a href="{WebUtility.HtmlEncode(url)}" style="display:inline-block;margin-top:18px;padding:13px 20px;background:#ee735b;color:#fff;text-decoration:none;border-radius:8px;font-weight:bold">{WebUtility.HtmlEncode(buttonText)}</a></div><div style="padding:18px 30px;background:#f3f0ea;color:#7a8496;font-size:12px">Bu mesaj otomatik olarak gönderilmiştir. Sorularınız için öğretmeninizle iletişime geçebilirsiniz.</div></div></body></html>""";
    }
}
