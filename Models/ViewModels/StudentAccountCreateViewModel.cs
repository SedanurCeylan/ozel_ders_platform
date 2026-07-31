using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentAccountCreateViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }=string.Empty;
    public string Email { get; set; }=string.Empty;
    [Required(ErrorMessage="Geçici şifre zorunludur."),MinLength(10,ErrorMessage="Geçici şifre en az 10 karakter olmalıdır."),DataType(DataType.Password),Display(Name="Geçici şifre")]public string TemporaryPassword { get; set; }=string.Empty;
}
