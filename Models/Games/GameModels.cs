using System.ComponentModel.DataAnnotations;
using OzelDersYonetim.Models.Students;

namespace OzelDersYonetim.Models.Games;

public enum GameType { OperationArena, GeometryExplorer, LevelTest }
public enum GameDifficulty { Easy, Medium, Hard }
public enum GameQuestionType { MultipleChoice, NumericInput, TrueFalse, Matching, DragDrop, ShapeSelection, CoordinateSelection }

public class GameQuestion
{
    public int Id { get; set; }
    public GameType GameType { get; set; } = GameType.OperationArena;
    [Range(5, 8), Display(Name = "Sınıf seviyesi")] public int GradeLevel { get; set; } = 5;
    [Required, StringLength(100), Display(Name = "Ders")] public string Subject { get; set; } = "Matematik";
    [Required, StringLength(120), Display(Name = "Konu")] public string Topic { get; set; } = string.Empty;
    [Display(Name = "Zorluk")] public GameDifficulty DifficultyLevel { get; set; }
    [Display(Name = "Soru türü")] public GameQuestionType QuestionType { get; set; }
    [Required, StringLength(1000), Display(Name = "Soru")] public string QuestionText { get; set; } = string.Empty;
    [StringLength(500)] public string? QuestionImagePath { get; set; }
    [Required, StringLength(250), Display(Name = "Doğru cevap")] public string CorrectAnswer { get; set; } = string.Empty;
    [Required, StringLength(1500), Display(Name = "Çözüm açıklaması")] public string Explanation { get; set; } = string.Empty;
    [Range(1, 1000), Display(Name = "Puan")] public int Score { get; set; } = 100;
    [Range(3, 300), Display(Name = "Süre (saniye)")] public int TimeLimitSeconds { get; set; } = 20;
    [Display(Name = "Aktif")] public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<GameQuestionOption> Options { get; set; } = new List<GameQuestionOption>();
}

public class GameQuestionOption
{
    public int Id { get; set; }
    public int GameQuestionId { get; set; }
    public GameQuestion GameQuestion { get; set; } = null!;
    [Required, StringLength(300)] public string OptionText { get; set; } = string.Empty;
    [StringLength(500)] public string? OptionImagePath { get; set; }
    public bool IsCorrect { get; set; }
    public int DisplayOrder { get; set; }
}

public class GameSession
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public GameType GameType { get; set; }
    public int GradeLevel { get; set; }
    [StringLength(100)] public string Subject { get; set; } = "Matematik";
    [StringLength(120)] public string Topic { get; set; } = string.Empty;
    public GameDifficulty DifficultyLevel { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int EmptyAnswers { get; set; }
    public int TotalScore { get; set; }
    public decimal AccuracyPercentage { get; set; }
    public decimal AverageAnswerTime { get; set; }
    public int LongestCorrectStreak { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSuspicious { get; set; }
    public ICollection<GameAnswer> Answers { get; set; } = new List<GameAnswer>();
}

public class GameAnswer
{
    public int Id { get; set; }
    public int GameSessionId { get; set; }
    public GameSession GameSession { get; set; } = null!;
    public int GameQuestionId { get; set; }
    public GameQuestion GameQuestion { get; set; } = null!;
    [StringLength(300)] public string StudentAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int EarnedScore { get; set; }
    public decimal AnswerTimeSeconds { get; set; }
    public int HintCount { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
