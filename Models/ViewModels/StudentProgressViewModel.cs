using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.ViewModels;

public class StudentProgressViewModel
{
    public StudentProfile Student { get; set; } = null!;
    public IReadOnlyList<ExamResult> ExamResults { get; set; } = Array.Empty<ExamResult>();
    public IReadOnlyList<StudentProgress> ProgressRecords { get; set; } = Array.Empty<StudentProgress>();
    public IReadOnlyList<TeacherStudentNote> TeacherNotes { get; set; } = Array.Empty<TeacherStudentNote>();
}
