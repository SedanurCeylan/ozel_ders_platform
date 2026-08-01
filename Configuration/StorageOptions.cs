namespace OzelDersYonetim.Configuration;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string RootPath { get; set; } = "App_Data/uploads";
}
