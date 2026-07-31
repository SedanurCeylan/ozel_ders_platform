using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
namespace OzelDersYonetim.Services.Notifications;
public class AnnouncementFileService(IWebHostEnvironment environment,IOptions<FileUploadOptions> options)
{
    private static readonly HashSet<string> Extensions=new(StringComparer.OrdinalIgnoreCase){".pdf",".jpg",".jpeg",".png"};
    private static readonly HashSet<string> MimeTypes=new(StringComparer.OrdinalIgnoreCase){"application/pdf","image/jpeg","image/png"};
    public async Task<string> SaveAsync(IFormFile file){var extension=Path.GetExtension(file.FileName);if(!Extensions.Contains(extension)||!MimeTypes.Contains(file.ContentType))throw new InvalidOperationException("Duyuru eki yalnızca PDF, JPG, JPEG veya PNG olabilir.");if(file.Length<=0||file.Length>options.Value.MaximumSizeMb*1024L*1024L)throw new InvalidOperationException($"Dosya boyutu en fazla {options.Value.MaximumSizeMb} MB olabilir.");var folder=Path.Combine(environment.ContentRootPath,"App_Data","uploads","announcements");Directory.CreateDirectory(folder);var stored=$"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";await using var stream=File.Create(Path.Combine(folder,stored));await file.CopyToAsync(stream);return stored;}
    public (Stream Stream,string ContentType,string FileName)? Open(string storedName){var safe=Path.GetFileName(storedName);if(safe!=storedName)return null;var path=Path.Combine(environment.ContentRootPath,"App_Data","uploads","announcements",safe);if(!File.Exists(path))return null;var ext=Path.GetExtension(safe).ToLowerInvariant();var contentType=ext switch{".pdf"=>"application/pdf",".png"=>"image/png",_=>"image/jpeg"};return(File.OpenRead(path),contentType,"duyuru-eki"+ext);}
}
