using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
namespace OzelDersYonetim.Services.Students;
public class ProfileImageService(IWebHostEnvironment environment,IOptions<FileUploadOptions> options)
{
    private static readonly HashSet<string> Extensions=new(StringComparer.OrdinalIgnoreCase){".jpg",".jpeg",".png"};
    private static readonly HashSet<string> MimeTypes=new(StringComparer.OrdinalIgnoreCase){"image/jpeg","image/png"};
    public async Task<string> SaveAsync(IFormFile file){var ext=Path.GetExtension(file.FileName);if(!Extensions.Contains(ext)||!MimeTypes.Contains(file.ContentType))throw new InvalidOperationException("Profil fotoğrafı JPG, JPEG veya PNG olmalıdır.");var maximum=Math.Min(options.Value.MaximumSizeMb,5)*1024L*1024L;if(file.Length<=0||file.Length>maximum)throw new InvalidOperationException("Profil fotoğrafı en fazla 5 MB olabilir.");var folder=Path.Combine(environment.ContentRootPath,"App_Data","uploads","profiles");Directory.CreateDirectory(folder);var stored=$"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";await using var stream=File.Create(Path.Combine(folder,stored));await file.CopyToAsync(stream);return stored;}
    public (Stream Stream,string ContentType)? Open(string storedName){var safe=Path.GetFileName(storedName);if(safe!=storedName)return null;var path=Path.Combine(environment.ContentRootPath,"App_Data","uploads","profiles",safe);if(!File.Exists(path))return null;return(File.OpenRead(path),Path.GetExtension(safe).Equals(".png",StringComparison.OrdinalIgnoreCase)?"image/png":"image/jpeg");}
}
