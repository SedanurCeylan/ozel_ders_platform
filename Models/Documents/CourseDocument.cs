using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OzelDersYonetim.Models.Documents;

public enum DocumentAccessType { Public, AllStudents, SelectedStudents }

public class CourseDocument
{
    public int Id { get; set; }
    [Required, StringLength(180), Display(Name = "Doküman başlığı")] public string Title { get; set; } = string.Empty;
    [StringLength(2000), Display(Name = "Açıklama")] public string? Description { get; set; }
    [Required, StringLength(80), Display(Name = "Kategori")] public string Category { get; set; } = "Genel";
    [Display(Name = "Erişim türü")] public DocumentAccessType AccessType { get; set; } = DocumentAccessType.AllStudents;
    [ValidateNever, Required, StringLength(255)] public string OriginalFileName { get; set; } = string.Empty;
    [ValidateNever, Required, StringLength(500)] public string StoredFilePath { get; set; } = string.Empty;
    [ValidateNever, Required, StringLength(150)] public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    [Display(Name = "Aktif")] public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    [ValidateNever] public ICollection<StudentDocument> StudentDocuments { get; set; } = new List<StudentDocument>();
    public string AccessName => AccessType switch { DocumentAccessType.Public => "Herkese açık", DocumentAccessType.AllStudents => "Tüm öğrencilere açık", _ => "Seçili öğrencilere özel" };
}
