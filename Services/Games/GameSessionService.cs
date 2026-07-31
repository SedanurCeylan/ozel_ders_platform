using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Services.Games;

public class GameSessionService(ApplicationDbContext db) : IGameSessionService
{
    public async Task<GameSession> StartArenaAsync(string userId, int grade, string topic, GameDifficulty difficulty, int durationSeconds)
    {
        if (grade is < 5 or > 8 || durationSeconds is not (30 or 45 or 60)) throw new InvalidOperationException("Oyun ayarları geçerli değil.");
        var student = await db.StudentProfiles.SingleOrDefaultAsync(x => x.ApplicationUserId == userId && x.IsActive) ?? throw new UnauthorizedAccessException();
        var hasQuestions = await db.GameQuestions.AnyAsync(x => x.IsActive && x.GameType == GameType.OperationArena && x.GradeLevel == grade && (topic == "Karışık" || x.Topic == topic));
        if (!hasQuestions) throw new InvalidOperationException("Bu seçimlere uygun aktif soru bulunamadı.");
        var session = new GameSession { StudentProfileId = student.Id, GameType = GameType.OperationArena, GradeLevel = grade, Topic = topic, DifficultyLevel = difficulty, DurationSeconds = durationSeconds };
        db.GameSessions.Add(session); await db.SaveChangesAsync(); return session;
    }

    public async Task<GameQuestionDto?> GetNextQuestionAsync(string userId, int sessionId)
    {
        var session = await OwnedSession(userId, sessionId);
        if (session.IsCompleted || DateTime.UtcNow > session.StartedAt.AddSeconds(session.DurationSeconds + 15)) return null;
        var used = await db.GameAnswers.Where(x => x.GameSessionId == sessionId).Select(x => x.GameQuestionId).ToListAsync();
        var targetDifficulty = AdaptiveDifficulty(session);
        var questions = await db.GameQuestions.AsNoTracking().Include(x => x.Options).Where(x => x.IsActive && x.GameType == session.GameType && x.GradeLevel == session.GradeLevel && (session.Topic == "Karışık" || x.Topic == session.Topic) && !used.Contains(x.Id)).ToListAsync();
        var question = questions.Where(x => x.DifficultyLevel == targetDifficulty).OrderBy(_ => Guid.NewGuid()).FirstOrDefault() ?? questions.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        return question is null ? null : ToDto(question, used.Count + 1);
    }

    public async Task<GameAnswerResult> AnswerAsync(string userId, GameAnswerRequest request)
    {
        var session = await OwnedSession(userId, request.SessionId);
        if (session.IsCompleted) throw new InvalidOperationException("Bu oyun oturumu zaten tamamlandı.");
        if (DateTime.UtcNow > session.StartedAt.AddSeconds(session.DurationSeconds + 15)) throw new InvalidOperationException("Oyun süresi sona erdi.");
        if (request.AnswerTimeSeconds is < 0.2m or > 180m) throw new InvalidOperationException("Cevap süresi geçerli değil.");
        if (await db.GameAnswers.AnyAsync(x => x.GameSessionId == request.SessionId && x.GameQuestionId == request.QuestionId)) throw new InvalidOperationException("Bu soru daha önce cevaplandı.");
        var question = await db.GameQuestions.Include(x => x.Options).SingleOrDefaultAsync(x => x.Id == request.QuestionId && x.IsActive && x.GameType == session.GameType && x.GradeLevel == session.GradeLevel && (session.Topic == "Karışık" || x.Topic == session.Topic)) ?? throw new InvalidOperationException("Soru bu oyun oturumuna ait değil.");
        var correct = Normalize(request.Answer) == Normalize(question.CorrectAnswer);
        var streak = await CurrentStreak(request.SessionId);
        var speedBonus = correct
            ? request.AnswerTimeSeconds <= 3m ? 50
            : request.AnswerTimeSeconds <= 6m ? 25
            : 0
            : 0;
        var streakBonus = correct && streak + 1 >= 5 ? 100 : correct && streak + 1 >= 3 ? 50 : 0;
        var earned = correct ? question.Score + speedBonus + streakBonus : 0;
        db.GameAnswers.Add(new GameAnswer { GameSessionId = session.Id, GameQuestionId = question.Id, StudentAnswer = request.Answer, IsCorrect = correct, EarnedScore = earned, AnswerTimeSeconds = request.AnswerTimeSeconds });
        session.TotalQuestions++; session.TotalScore += earned; if (correct) session.CorrectAnswers++; else session.WrongAnswers++;
        await db.SaveChangesAsync();
        var next = await GetNextQuestionAsync(userId, session.Id);
        return new GameAnswerResult(correct, question.CorrectAnswer, question.Explanation, earned, session.TotalScore, session.CorrectAnswers, session.WrongAnswers, correct ? streak + 1 : 0, next, next is null);
    }

    public async Task<GameSession> CompleteAsync(string userId, int sessionId)
    {
        var session = await OwnedSession(userId, sessionId);
        if (session.IsCompleted) return session;
        var answers = await db.GameAnswers.Where(x => x.GameSessionId == sessionId).OrderBy(x => x.AnsweredAt).ToListAsync();
        session.IsCompleted = true; session.CompletedAt = DateTime.UtcNow; session.TotalQuestions = answers.Count; session.CorrectAnswers = answers.Count(x => x.IsCorrect); session.WrongAnswers = answers.Count(x => !x.IsCorrect); session.AccuracyPercentage = answers.Count == 0 ? 0 : Math.Round(session.CorrectAnswers * 100m / answers.Count, 1); session.AverageAnswerTime = answers.Count == 0 ? 0 : Math.Round(answers.Average(x => x.AnswerTimeSeconds), 1); session.LongestCorrectStreak = LongestStreak(answers); session.IsSuspicious = answers.Count >= 5 && session.AverageAnswerTime < .8m;
        await db.SaveChangesAsync(); return session;
    }

    public async Task<GameSession?> GetOwnedResultAsync(string userId, int sessionId) => await db.GameSessions.AsNoTracking().Include(x => x.Answers).ThenInclude(x => x.GameQuestion).SingleOrDefaultAsync(x => x.Id == sessionId && x.StudentProfile.ApplicationUserId == userId);
    private async Task<GameSession> OwnedSession(string userId, int id) => await db.GameSessions.SingleOrDefaultAsync(x => x.Id == id && x.StudentProfile.ApplicationUserId == userId) ?? throw new UnauthorizedAccessException();
    private async Task<int> CurrentStreak(int id) { var values = await db.GameAnswers.Where(x => x.GameSessionId == id).OrderByDescending(x => x.AnsweredAt).Select(x => x.IsCorrect).ToListAsync(); var count = 0; foreach (var value in values) { if (!value) break; count++; } return count; }
    private static GameDifficulty AdaptiveDifficulty(GameSession s) => s.CorrectAnswers >= 3 && s.CorrectAnswers - s.WrongAnswers >= 2 ? GameDifficulty.Hard : s.WrongAnswers >= 2 && s.WrongAnswers > s.CorrectAnswers ? GameDifficulty.Easy : s.DifficultyLevel;
    private static string Normalize(string value) => value.Trim().Replace(',', '.').ToUpperInvariant();
    private static GameQuestionDto ToDto(GameQuestion q, int number) => new(q.Id, q.QuestionText, q.QuestionType.ToString(), q.Options.OrderBy(_ => Guid.NewGuid()).Select(x => x.OptionText).ToList(), number, q.Score);
    private static int LongestStreak(IEnumerable<GameAnswer> answers) { var best = 0; var current = 0; foreach (var a in answers) { current = a.IsCorrect ? current + 1 : 0; best = Math.Max(best, current); } return best; }
}
