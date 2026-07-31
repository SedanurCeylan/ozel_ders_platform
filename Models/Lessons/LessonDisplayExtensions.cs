namespace OzelDersYonetim.Models.Lessons;

public static class LessonDisplayExtensions
{
    public static string ToTurkish(this LessonStatus status) => status switch
    {
        LessonStatus.Planned => "Planlandı",
        LessonStatus.Confirmed => "Onaylandı",
        LessonStatus.Completed => "Tamamlandı",
        LessonStatus.TeacherCancelled => "Öğretmen iptal etti",
        LessonStatus.StudentCancelled => "Öğrenci iptal etti",
        LessonStatus.Postponed => "Ertelendi",
        _ => "Bilinmiyor"
    };

    public static string ToTurkish(this AttendanceStatus status) => status switch
    {
        AttendanceStatus.Pending => "Bekliyor",
        AttendanceStatus.Attended => "Katıldı",
        AttendanceStatus.Absent => "Katılmadı",
        AttendanceStatus.Excused => "Mazeretli",
        _ => "Bilinmiyor"
    };

    public static string ToTurkish(this LessonMode mode) => mode switch
    {
        LessonMode.Online => "Çevrim içi",
        LessonMode.FaceToFace => "Yüz yüze",
        _ => "Bilinmiyor"
    };
}
