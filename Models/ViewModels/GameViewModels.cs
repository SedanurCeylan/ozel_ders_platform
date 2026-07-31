using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Models.ViewModels;

public class GamesHomeViewModel
{
    public int DefaultGrade { get; set; }
    public IReadOnlyList<GameSession> RecentSessions { get; set; } = Array.Empty<GameSession>();
    public int TotalGames { get; set; }
    public int TotalScore { get; set; }
    public decimal Accuracy { get; set; }
}

public record GameQuestionDto(int Id, string Text, string Type, IReadOnlyList<string> Options, int Number, int Score);
public record GameAnswerRequest(int SessionId, int QuestionId, string Answer, decimal AnswerTimeSeconds);
public record GameAnswerResult(bool IsCorrect, string CorrectAnswer, string Explanation, int EarnedScore, int TotalScore, int Correct, int Wrong, int Streak, GameQuestionDto? NextQuestion, bool Finished);

public class GameResultViewModel
{
    public GameSession Session { get; set; } = null!;
    public int PreviousBest { get; set; }
    public bool IsNewRecord => Session.TotalScore > PreviousBest;
}
