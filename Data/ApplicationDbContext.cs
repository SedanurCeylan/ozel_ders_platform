using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.Students;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.Notifications;
using OzelDersYonetim.Models.Auditing;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    public DbSet<ContentSection> ContentSections => Set<ContentSection>();
    public DbSet<DailyFact> DailyFacts => Set<DailyFact>();
    public DbSet<StudentTestimonial> StudentTestimonials => Set<StudentTestimonial>();

    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<StudentLesson> StudentLessons => Set<StudentLesson>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<StudentAssignment> StudentAssignments => Set<StudentAssignment>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<CourseDocument> CourseDocuments => Set<CourseDocument>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<StudentProgress> StudentProgressRecords => Set<StudentProgress>();
    public DbSet<TeacherStudentNote> TeacherStudentNotes => Set<TeacherStudentNote>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementStudent> AnnouncementStudents => Set<AnnouncementStudent>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<ReminderDispatch> ReminderDispatches => Set<ReminderDispatch>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<GameQuestion> GameQuestions => Set<GameQuestion>();
    public DbSet<GameQuestionOption> GameQuestionOptions => Set<GameQuestionOption>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<GameAnswer> GameAnswers => Set<GameAnswer>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ContentSection>()
            .HasIndex(section => new { section.PageKey, section.SectionKey })
            .IsUnique();

        builder.Entity<StudentTestimonial>().HasIndex(x=>x.StudentProfileId).IsUnique();
        builder.Entity<StudentTestimonial>().HasQueryFilter(x => !x.StudentProfile.IsDeleted);
        builder.Entity<StudentTestimonial>().HasOne(x=>x.StudentProfile).WithMany().HasForeignKey(x=>x.StudentProfileId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentProfile>()
            .HasIndex(student => student.Email)
            .IsUnique();

        builder.Entity<StudentProfile>()
            .HasQueryFilter(student => !student.IsDeleted);

        builder.Entity<StudentProfile>()
            .HasOne(student => student.ApplicationUser)
            .WithOne(user => user.StudentProfile)
            .HasForeignKey<StudentProfile>(student => student.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentLesson>().HasIndex(item => new { item.LessonId, item.StudentProfileId }).IsUnique();
        builder.Entity<StudentLesson>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<StudentLesson>().HasOne(item => item.Lesson).WithMany(item => item.StudentLessons).HasForeignKey(item => item.LessonId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<StudentLesson>().HasOne(item => item.StudentProfile).WithMany(item => item.StudentLessons).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentAssignment>().HasIndex(item => new { item.AssignmentId, item.StudentProfileId }).IsUnique();
        builder.Entity<StudentAssignment>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<StudentAssignment>().HasOne(item => item.Assignment).WithMany(item => item.StudentAssignments).HasForeignKey(item => item.AssignmentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<StudentAssignment>().HasOne(item => item.StudentProfile).WithMany(item => item.StudentAssignments).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AssignmentSubmission>().HasOne(item => item.StudentAssignment).WithMany(item => item.Submissions).HasForeignKey(item => item.StudentAssignmentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AssignmentSubmission>().HasQueryFilter(item => !item.StudentAssignment.StudentProfile.IsDeleted);
        builder.Entity<StudentDocument>().HasIndex(item => new { item.CourseDocumentId, item.StudentProfileId }).IsUnique();
        builder.Entity<StudentDocument>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<StudentDocument>().HasOne(item => item.CourseDocument).WithMany(item => item.StudentDocuments).HasForeignKey(item => item.CourseDocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<StudentDocument>().HasOne(item => item.StudentProfile).WithMany(item => item.StudentDocuments).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ExamResult>().HasOne(item => item.StudentProfile).WithMany(item => item.ExamResults).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ExamResult>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<StudentProgress>().HasOne(item => item.StudentProfile).WithMany(item => item.ProgressRecords).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<StudentProgress>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<TeacherStudentNote>().HasOne(item => item.StudentProfile).WithMany(item => item.TeacherNotes).HasForeignKey(item => item.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TeacherStudentNote>().HasQueryFilter(item => !item.StudentProfile.IsDeleted);
        builder.Entity<AnnouncementStudent>().HasIndex(x => new { x.AnnouncementId, x.StudentProfileId }).IsUnique();
        builder.Entity<AnnouncementStudent>().HasQueryFilter(x => !x.StudentProfile.IsDeleted);
        builder.Entity<AnnouncementStudent>().HasOne(x => x.Announcement).WithMany(x => x.AnnouncementStudents).HasForeignKey(x => x.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AnnouncementStudent>().HasOne(x => x.StudentProfile).WithMany(x => x.AnnouncementStudents).HasForeignKey(x => x.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<UserNotification>().HasOne(x => x.ApplicationUser).WithMany(x => x.Notifications).HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ReminderDispatch>().HasIndex(x => new { x.ReminderType, x.EntityType, x.EntityId, x.ApplicationUserId }).IsUnique();
        builder.Entity<GameQuestionOption>().HasOne(x => x.GameQuestion).WithMany(x => x.Options).HasForeignKey(x => x.GameQuestionId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameSession>().HasOne(x => x.StudentProfile).WithMany().HasForeignKey(x => x.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<GameSession>().HasQueryFilter(x => x.StudentProfile == null || !x.StudentProfile.IsDeleted);
        builder.Entity<GameAnswer>().HasIndex(x => new { x.GameSessionId, x.GameQuestionId }).IsUnique();
        builder.Entity<GameAnswer>().HasQueryFilter(x => x.GameSession.StudentProfile == null || !x.GameSession.StudentProfile.IsDeleted);
        builder.Entity<GameAnswer>().HasOne(x => x.GameSession).WithMany(x => x.Answers).HasForeignKey(x => x.GameSessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameAnswer>().HasOne(x => x.GameQuestion).WithMany().HasForeignKey(x => x.GameQuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
