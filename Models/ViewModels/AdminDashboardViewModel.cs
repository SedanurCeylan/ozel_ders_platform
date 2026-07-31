using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Notifications;
using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int PassiveStudents { get; set; }
    public int TodayLessons { get; set; }
    public int UpcomingLessons { get; set; }
    public int PendingAssignments { get; set; }
    public int LateAssignments { get; set; }
    public int AwaitingEvaluation { get; set; }
    public int FailedEmails { get; set; }
    public int TotalDocuments { get; set; }
    public int MonthlyLessons { get; set; }
    public int UnreadAnnouncements { get; set; }
    public IReadOnlyList<Lesson> TodaySchedule { get; set; } = Array.Empty<Lesson>();
    public IReadOnlyList<StudentProfile> RecentStudents { get; set; } = Array.Empty<StudentProfile>();
    public IReadOnlyList<StudentAssignment> UpcomingAssignmentDeadlines { get; set; } = Array.Empty<StudentAssignment>();
    public IReadOnlyList<AssignmentSubmission> RecentSubmissions { get; set; } = Array.Empty<AssignmentSubmission>();
    public IReadOnlyList<ExamResult> RecentExamResults { get; set; } = Array.Empty<ExamResult>();
    public IReadOnlyList<EmailLog> RecentEmailErrors { get; set; } = Array.Empty<EmailLog>();
}
