using System.ComponentModel.DataAnnotations;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentEmailViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }=string.Empty;
    public string StudentEmail { get; set; }=string.Empty;
    public string? ParentName { get; set; }
    public string? ParentEmail { get; set; }
    [Required,Display(Name="Alıcı")]public string RecipientType { get; set; }="Student";
    [Required(ErrorMessage="Konu zorunludur."),StringLength(250),Display(Name="E-posta konusu")]public string Subject { get; set; }=string.Empty;
    [Required(ErrorMessage="Mesaj zorunludur."),StringLength(6000),Display(Name="Mesaj")]public string Message { get; set; }=string.Empty;
}
