using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentPasswordResetViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }=string.Empty;
    [Required(ErrorMessage="Geçici şifre zorunludur."),MinLength(10,ErrorMessage="Geçici şifre en az 10 karakter olmalıdır."),DataType(DataType.Password),Display(Name="Yeni geçici şifre")] public string TemporaryPassword { get; set; }=string.Empty;
}
