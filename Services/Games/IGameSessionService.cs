using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Services.Games;

public interface IGameSessionService
{
    Task<GameSession> StartArenaAsync(string userId, int grade, string topic, GameDifficulty difficulty, int durationSeconds);
    Task<GameQuestionDto?> GetNextQuestionAsync(string userId, int sessionId);
    Task<GameAnswerResult> AnswerAsync(string userId, GameAnswerRequest request);
    Task<GameSession> CompleteAsync(string userId, int sessionId);
    Task<GameSession?> GetOwnedResultAsync(string userId, int sessionId);
}
