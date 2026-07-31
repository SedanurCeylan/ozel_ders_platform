using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Models.Identity;

namespace OzelDersYonetim.Data;

public sealed class IdentityDataSeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<AdminSeedOptions> options,
    ILogger<IdentityDataSeeder> logger)
{
    public const string AdminRole = "Admin";
    public const string StudentRole = "Student";

    public async Task SeedAsync()
    {
        foreach (var roleName in new[] { AdminRole, StudentRole })
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            EnsureSucceeded(roleResult, $"{roleName} rolü oluşturulamadı");
        }

        var adminOptions = options.Value;
        if (string.IsNullOrWhiteSpace(adminOptions.Email) ||
            string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            logger.LogWarning(
                "Admin hesabı oluşturulmadı. AdminSeed:Email ve AdminSeed:Password değerlerini User Secrets veya ortam değişkeni ile tanımlayın.");
            return;
        }

        var admin = await userManager.FindByEmailAsync(adminOptions.Email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminOptions.Email,
                Email = adminOptions.Email,
                EmailConfirmed = true,
                FirstName = adminOptions.FirstName,
                LastName = adminOptions.LastName,
                IsActive = true,
                MustChangePassword = true
            };

            var createResult = await userManager.CreateAsync(admin, adminOptions.Password);
            EnsureSucceeded(createResult, "Başlangıç admin hesabı oluşturulamadı");
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, AdminRole);
            EnsureSucceeded(addRoleResult, "Admin rolü başlangıç kullanıcısına atanamadı");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message}: {errors}");
    }
}
