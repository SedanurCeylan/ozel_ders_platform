using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OzelDersYonetim.Models.Identity;

namespace OzelDersYonetim.Areas.Identity.Pages.Account;

public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (user.MustChangePassword)
            {
                return LocalRedirect("/Identity/Account/Manage/ChangePassword");
            }
            if (await userManager.IsInRoleAsync(user, "Admin") && returnUrl == Url.Content("~/"))
            {
                returnUrl = Url.Content("~/Admin");
            }
            else if (await userManager.IsInRoleAsync(user, "Student") && returnUrl == Url.Content("~/"))
            {
                returnUrl = Url.Content("~/Student");
            }

            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Çok fazla hatalı deneme yapıldı. Lütfen 15 dakika sonra tekrar deneyin.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi yazın.")]
        [Display(Name = "E-posta adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni hatırla")]
        public bool RememberMe { get; set; }
    }
}
