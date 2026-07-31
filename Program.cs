using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Services.Assignments;
using OzelDersYonetim.Services.Documents;
using OzelDersYonetim.Services.Notifications;
using OzelDersYonetim.Services.Auditing;
using OzelDersYonetim.Services.Students;
using OzelDersYonetim.Services.Games;

namespace OzelDersYonetim;

public class Program
{
    public static async Task Main(string[] args)
    {
        var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture = turkishCulture;
        CultureInfo.DefaultThreadCurrentUICulture = turkishCulture;

        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.Configure<AdminSeedOptions>(
            builder.Configuration.GetSection(AdminSeedOptions.SectionName));
        builder.Services.Configure<FileUploadOptions>(builder.Configuration.GetSection(FileUploadOptions.SectionName));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
        builder.Services.Configure<ReminderOptions>(builder.Configuration.GetSection(ReminderOptions.SectionName));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });
        builder.Services.AddScoped<IdentityDataSeeder>();
        builder.Services.AddScoped<SiteContentDataSeeder>();
        builder.Services.AddScoped<AssignmentFileService>();
        builder.Services.AddScoped<IAssignmentService, AssignmentService>();
        builder.Services.AddScoped<IAssignmentSubmissionService, AssignmentSubmissionService>();
        builder.Services.AddScoped<IStudentDocumentService, StudentDocumentService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
        builder.Services.AddScoped<AnnouncementFileService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<ProfileImageService>();
        builder.Services.AddScoped<IGameSessionService, GameSessionService>();
        builder.Services.AddScoped<GameDataSeeder>();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<PublicGameService>();
        builder.Services.AddHostedService<ReminderBackgroundService>();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        if (args.Contains("--seed-games-only", StringComparer.OrdinalIgnoreCase))
        {
            using var gameSeedScope = app.Services.CreateScope();
            await gameSeedScope.ServiceProvider.GetRequiredService<GameDataSeeder>().SeedAsync();
            return;
        }

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
            await scope.ServiceProvider.GetRequiredService<IdentityDataSeeder>().SeedAsync();
            await scope.ServiceProvider.GetRequiredService<SiteContentDataSeeder>().SeedAsync();
            await scope.ServiceProvider.GetRequiredService<GameDataSeeder>().SeedAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
        app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");
        app.UseStaticFiles();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.XFrameOptions = "DENY";
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
            await next();
        });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}
