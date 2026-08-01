using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Models.ViewModels;

public class PublicSiteViewModel
{
    public SiteSetting Settings { get; set; } = null!;
    public IReadOnlyList<ContentSection> Sections { get; set; } = Array.Empty<ContentSection>();
    public IReadOnlyList<CourseDocument> PublicDocuments { get; set; } = Array.Empty<CourseDocument>();
    public DailyFact? DailyFact { get; set; }
    public DateOnly LocalToday { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public IReadOnlyList<StudentTestimonial> StudentTestimonials { get; set; } = Array.Empty<StudentTestimonial>();
    public IReadOnlyList<ContentSection> ClassContents { get; set; } = Array.Empty<ContentSection>();
    public ContentSection? Find(string key) => Sections.FirstOrDefault(x => x.SectionKey == key);
    public IReadOnlyList<ContentSection> Items(string prefix) => Sections.Where(x => x.SectionKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.DisplayOrder).ToList();
}
