using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.Notifications;

namespace OzelDersYonetim.Models.Students;

public class StudentProfile
{
    public int Id { get; set; }

    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    [Required, StringLength(80), Display(Name = "Ad")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80), Display(Name = "Soyad")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180), Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(40), Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [DataType(DataType.Date), Display(Name = "Doğum tarihi")]
    public DateTime? BirthDate { get; set; }

    [Required, StringLength(40), Display(Name = "Sınıf seviyesi")]
    public string GradeLevel { get; set; } = string.Empty;

    [StringLength(160), Display(Name = "Okul adı")]
    public string? SchoolName { get; set; }

    [StringLength(80), Display(Name = "Veli adı")]
    public string? ParentFirstName { get; set; }

    [StringLength(80), Display(Name = "Veli soyadı")]
    public string? ParentLastName { get; set; }

    [Phone, StringLength(40), Display(Name = "Veli telefonu")]
    public string? ParentPhone { get; set; }

    [EmailAddress, StringLength(180), Display(Name = "Veli e-postası")]
    public string? ParentEmail { get; set; }

    [Required, StringLength(80), Display(Name = "Ders türü")]
    public string LessonType { get; set; } = "Matematik";

    [Required, StringLength(40), Display(Name = "Ders tercihi")]
    public string LessonPreference { get; set; } = "Online";

    [StringLength(2000), Display(Name = "Öğretmen notu")]
    public string? TeacherNote { get; set; }

    [StringLength(500)]
    public string? ProfileImagePath { get; set; }

    [DataType(DataType.Date), Display(Name = "Kayıt tarihi")]
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow.Date;

    [Display(Name = "Aktif öğrenci")]
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<StudentLesson> StudentLessons { get; set; } = new List<StudentLesson>();
    public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
    public ICollection<StudentDocument> StudentDocuments { get; set; } = new List<StudentDocument>();
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    public ICollection<StudentProgress> ProgressRecords { get; set; } = new List<StudentProgress>();
    public ICollection<TeacherStudentNote> TeacherNotes { get; set; } = new List<TeacherStudentNote>();
    public ICollection<AnnouncementStudent> AnnouncementStudents { get; set; } = new List<AnnouncementStudent>();

    public string FullName => $"{FirstName} {LastName}";
}
