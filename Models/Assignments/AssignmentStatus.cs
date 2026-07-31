namespace OzelDersYonetim.Models.Assignments;

public enum AssignmentStatus { Draft, Published, InProgress, AwaitingSubmission, Completed, Expired, Cancelled }
public enum StudentAssignmentStatus { Assigned, Viewed, InProgress, Submitted, LateSubmitted, Evaluated, ResubmissionRequested, Completed }

public static class AssignmentDisplayExtensions
{
    public static string ToTurkish(this AssignmentStatus status) => status switch { AssignmentStatus.Draft => "Taslak", AssignmentStatus.Published => "Yayında", AssignmentStatus.InProgress => "Devam ediyor", AssignmentStatus.AwaitingSubmission => "Teslim bekliyor", AssignmentStatus.Completed => "Tamamlandı", AssignmentStatus.Expired => "Süresi doldu", AssignmentStatus.Cancelled => "İptal edildi", _ => "Bilinmiyor" };
    public static string ToTurkish(this StudentAssignmentStatus status) => status switch { StudentAssignmentStatus.Assigned => "Atandı", StudentAssignmentStatus.Viewed => "Görüntülendi", StudentAssignmentStatus.InProgress => "Devam ediyor", StudentAssignmentStatus.Submitted => "Teslim edildi", StudentAssignmentStatus.LateSubmitted => "Geç teslim edildi", StudentAssignmentStatus.Evaluated => "Değerlendirildi", StudentAssignmentStatus.ResubmissionRequested => "Yeniden teslim istendi", StudentAssignmentStatus.Completed => "Tamamlandı", _ => "Bilinmiyor" };
}
