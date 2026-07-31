namespace OzelDersYonetim.Configuration;

public class FileUploadOptions
{
    public const string SectionName = "FileUploads";
    public int MaximumSizeMb { get; set; } = 20;
}
