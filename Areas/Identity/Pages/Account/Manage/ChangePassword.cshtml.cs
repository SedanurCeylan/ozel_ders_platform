using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OzelDersYonetim.Models.Identity;

namespace OzelDersYonetim.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = await userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var result = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, TranslateError(error.Code));
            return Page();
        }

        user.MustChangePassword = false;
        user.UpdatedAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "Şifreniz başarıyla değiştirildi.";
        return RedirectToPage();
    }

    private static string TranslateError(string code) => code switch
    {
        "PasswordMismatch" => "Mevcut şifreniz hatalı.",
        "PasswordTooShort" => "Yeni şifre en az 10 karakter olmalıdır.",
        "PasswordRequiresDigit" => "Yeni şifre en az bir rakam içermelidir.",
        "PasswordRequiresUpper" => "Yeni şifre en az bir büyük harf içermelidir.",
        "PasswordRequiresLower" => "Yeni şifre en az bir küçük harf içermelidir.",
        "PasswordRequiresNonAlphanumeric" => "Yeni şifre en az bir özel karakter içermelidir.",
        _ => "Şifre değiştirilemedi. Lütfen bilgileri kontrol edin."
    };

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunludur."), DataType(DataType.Password), Display(Name = "Mevcut şifre")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre zorunludur."), StringLength(100, MinimumLength = 10, ErrorMessage = "Şifre en az 10 karakter olmalıdır."), DataType(DataType.Password), Display(Name = "Yeni şifre")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreler eşleşmiyor."), Display(Name = "Yeni şifre tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
