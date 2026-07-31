using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Notifications;
using OzelDersYonetim.Models.Students;
namespace OzelDersYonetim.Models.ViewModels;
public class StudentDashboardViewModel
{
    public StudentProfile Student { get; set; }=null!;
    public IReadOnlyList<StudentLesson> UpcomingLessons { get; set; }=Array.Empty<StudentLesson>();
    public IReadOnlyList<StudentAssignment> UpcomingAssignments { get; set; }=Array.Empty<StudentAssignment>();
    public IReadOnlyList<StudentAssignment> RecentFeedback { get; set; }=Array.Empty<StudentAssignment>();
    public IReadOnlyList<Announcement> RecentAnnouncements { get; set; }=Array.Empty<Announcement>();
    public IReadOnlyList<CourseDocument> RecentDocuments { get; set; }=Array.Empty<CourseDocument>();
    public int LateAssignmentCount { get; set; }
    public int EvaluatedAssignmentCount { get; set; }
    public int UnreadNotificationCount { get; set; }
    public int DocumentCount { get; set; }
}
