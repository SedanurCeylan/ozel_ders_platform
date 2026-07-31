using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Notifications;
using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentDetailViewModel
{
    public StudentProfile Student { get; set; }=null!;
    public IReadOnlyList<StudentLesson> Lessons { get; set; }=Array.Empty<StudentLesson>();
    public IReadOnlyList<StudentAssignment> Assignments { get; set; }=Array.Empty<StudentAssignment>();
    public IReadOnlyList<StudentDocument> Documents { get; set; }=Array.Empty<StudentDocument>();
    public IReadOnlyList<ExamResult> ExamResults { get; set; }=Array.Empty<ExamResult>();
    public IReadOnlyList<StudentProgress> ProgressRecords { get; set; }=Array.Empty<StudentProgress>();
    public IReadOnlyList<TeacherStudentNote> TeacherNotes { get; set; }=Array.Empty<TeacherStudentNote>();
    public IReadOnlyList<EmailLog> EmailLogs { get; set; }=Array.Empty<EmailLog>();
    public decimal AttendanceRate { get; set; }
    public decimal AssignmentSuccessRate { get; set; }
}
