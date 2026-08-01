using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Models.ViewModels;

public class PublicCollectionViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<ContentSection> Items { get; set; } = Array.Empty<ContentSection>();
    public IReadOnlyList<CourseDocument> Documents { get; set; } = Array.Empty<CourseDocument>();
}

public class PublicArticleViewModel
{
    public ContentSection Article { get; set; } = new();
    public IReadOnlyList<(string Heading, string Body)> Sections { get; set; } = Array.Empty<(string, string)>();
    public IReadOnlyList<ContentSection> RelatedArticles { get; set; } = Array.Empty<ContentSection>();
}

public class LevelTestViewModel
{
    public string Token { get; set; } = string.Empty;
    public int Grade { get; set; }
    public IReadOnlyList<GameQuestion> Questions { get; set; } = Array.Empty<GameQuestion>();
}

public record LevelTestResultViewModel(int Grade,int Correct,int Wrong,decimal Accuracy,IReadOnlyList<string> StrongTopics,IReadOnlyList<string> DevelopmentTopics,string Recommendation);
