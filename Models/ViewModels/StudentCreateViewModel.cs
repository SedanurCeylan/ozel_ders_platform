using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;
using Microsoft.AspNetCore.Http;

namespace OzelDersYonetim.Models.ViewModels;

public class StudentCreateViewModel
{
    public StudentProfile Student { get; set; } = new();

    [Display(Name = "Öğrenci için giriş hesabı oluştur")]
    public bool CreateAccount { get; set; } = true;

    [DataType(DataType.Password), Display(Name = "Geçici şifre")]
    public string? TemporaryPassword { get; set; }
    [Display(Name="Profil fotoğrafı")] public IFormFile? ProfileImage { get; set; }
}
