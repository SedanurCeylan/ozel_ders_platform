namespace OzelDersYonetim.Configuration;

public sealed class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    public string? Email { get; init; }

    public string? Password { get; init; }

    public string FirstName { get; init; } = "Öğretmen";

    public string LastName { get; init; } = "Admin";
}
