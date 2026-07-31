using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OzelDersYonetim.Models.Identity;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;

namespace OzelDersYonetim.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string Email { get; private set; } = string.Empty;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return NotFound();
        Email = user.Email ?? string.Empty;
        var student = User.IsInRole(IdentityDataSeeder.StudentRole) ? await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.ApplicationUserId==user.Id) : null;
        Input = new InputModel { FirstName = student?.FirstName??user.FirstName, LastName = student?.LastName??user.LastName, PhoneNumber = student?.Phone??user.PhoneNumber, BirthDate=student?.BirthDate, SchoolName=student?.SchoolName, LessonPreference=student?.LessonPreference };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return NotFound();
        Email = user.Email ?? string.Empty;
        if (!ModelState.IsValid) return Page();

        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.PhoneNumber = Input.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        if (User.IsInRole(IdentityDataSeeder.StudentRole))
        {
            var student = await dbContext.StudentProfiles.SingleOrDefaultAsync(x=>x.ApplicationUserId==user.Id);
            if(student is null)return NotFound();
            student.FirstName=Input.FirstName?.Trim()??student.FirstName;student.LastName=Input.LastName?.Trim()??student.LastName;student.Phone=Input.PhoneNumber?.Trim();student.BirthDate=Input.BirthDate;student.SchoolName=Input.SchoolName?.Trim();
            if(Input.LessonPreference is "Online" or "Yüz yüze" or "Hibrit")student.LessonPreference=Input.LessonPreference;
            student.UpdatedAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "Hesap bilgileriniz güncellendi.";
        return RedirectToPage();
    }

    public sealed class InputModel
    {
        [StringLength(80), Display(Name = "Ad")]
        public string? FirstName { get; set; }

        [StringLength(80), Display(Name = "Soyad")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası yazın."), Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date), Display(Name="Doğum tarihi")] public DateTime? BirthDate { get; set; }
        [StringLength(160), Display(Name="Okul adı")] public string? SchoolName { get; set; }
        [StringLength(40), Display(Name="Ders tercihi")] public string? LessonPreference { get; set; }
    }
}
