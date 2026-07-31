using OzelDersYonetim.Models.Notifications;
namespace OzelDersYonetim.Services.Notifications;
public interface IAnnouncementService { Task PublishAsync(Announcement announcement, IReadOnlyCollection<int> selectedStudentIds); }
