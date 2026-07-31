using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentListItemViewModel { public StudentProfile Student { get; set; }=null!; public DateTime? LastLessonDate { get; set; } public int PendingAssignmentCount { get; set; } }
