using Microsoft.AspNetCore.Identity;

namespace OzelDersYonetim.Models.Identity;

public static class IdentityErrorTranslator
{
    public static string ToTurkish(this IdentityError error) => error.Code switch
    {
        "DuplicateEmail" => "Bu e-posta adresi başka bir hesap tarafından kullanılıyor.",
        "DuplicateUserName" => "Bu e-posta adresi başka bir hesap tarafından kullanılıyor.",
        "InvalidEmail" => "Geçerli bir e-posta adresi girin.",
        "InvalidUserName" => "E-posta adresinde geçersiz karakterler bulunuyor.",
        "PasswordTooShort" => "Şifre en az 10 karakter olmalıdır.",
        "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
        "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
        "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
        "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
        "PasswordMismatch" => "Girilen şifre hatalı.",
        "UserAlreadyInRole" => "Kullanıcı bu role zaten atanmış.",
        _ => "İşlem tamamlanamadı. Bilgileri kontrol edip yeniden deneyin."
    };
}
