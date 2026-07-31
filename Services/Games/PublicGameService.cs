using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Services.Games;

public sealed class PublicGameService(ApplicationDbContext db, IMemoryCache cache)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(20);

    public async Task<(string Token, PublicGameQuestion Question, DateTime EndsAt)> StartAsync(int grade, string topic, GameDifficulty difficulty, int durationSeconds)
    {
        if (grade is < 5 or > 8 || durationSeconds is not (30 or 45 or 60)) throw new InvalidOperationException("Oyun ayarları geçerli değil.");
        var state = new State { Grade=grade, Topic=topic, Difficulty=difficulty, DurationSeconds=durationSeconds, StartedAt=DateTime.UtcNow };
        var question = await NextAsync(state) ?? throw new InvalidOperationException("Bu seçimlere uygun aktif soru bulunamadı.");
        state.CurrentQuestionId=question.Id;
        var token=Guid.NewGuid().ToString("N");cache.Set(Key(token),state,Lifetime);
        return (token,question,state.StartedAt.AddSeconds(durationSeconds));
    }

    public (PublicGameQuestion Question, DateTime EndsAt) Current(string token)
    {
        var state=Get(token);return (state.CurrentQuestion??throw new InvalidOperationException("Oyun sorusu bulunamadı."),state.StartedAt.AddSeconds(state.DurationSeconds));
    }

    public async Task<PublicGameAnswer> AnswerAsync(string token,int questionId,string answer,decimal answerTimeSeconds)
    {
        var state=Get(token);
        if(state.Completed||DateTime.UtcNow>state.StartedAt.AddSeconds(state.DurationSeconds+10))throw new InvalidOperationException("Oyun süresi sona erdi.");
        if(questionId!=state.CurrentQuestionId||state.Used.Contains(questionId))throw new InvalidOperationException("Bu soru artık cevaplanamaz.");
        if(answerTimeSeconds is < .2m or > 180m)throw new InvalidOperationException("Cevap süresi geçerli değil.");
        var q=await db.GameQuestions.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==questionId&&x.IsActive&&x.GameType==GameType.OperationArena)??throw new InvalidOperationException("Soru bulunamadı.");
        var correct=Normalize(answer)==Normalize(q.CorrectAnswer);var speed=correct?(answerTimeSeconds<=3?50:answerTimeSeconds<=6?25:0):0;
        state.Streak=correct?state.Streak+1:0;state.Longest=Math.Max(state.Longest,state.Streak);var streak=correct&&state.Streak>=5?100:correct&&state.Streak>=3?50:0;var earned=correct?q.Score+speed+streak:0;
        state.Score+=earned;if(correct)state.Correct++;else state.Wrong++;state.TotalSeconds+=answerTimeSeconds;state.Used.Add(questionId);
        var next=await NextAsync(state);state.CurrentQuestion=next;state.CurrentQuestionId=next?.Id??0;state.Completed=next is null;cache.Set(Key(token),state,Lifetime);
        return new(correct,q.CorrectAnswer,q.Explanation,earned,state.Score,state.Correct,state.Wrong,state.Streak,next,next is null);
    }

    public PublicGameResult Complete(string token)
    {
        var s=Get(token);s.Completed=true;cache.Set(Key(token),s,TimeSpan.FromMinutes(10));var total=s.Correct+s.Wrong;
        return new(s.Score,s.Correct,s.Wrong,total==0?0:Math.Round(s.Correct*100m/total,1),total==0?0:Math.Round(s.TotalSeconds/total,1),s.Longest,s.Grade,s.Topic,s.Difficulty,s.DurationSeconds);
    }

    private State Get(string token)=>cache.TryGetValue(Key(token),out State? state)&&state is not null?state:throw new InvalidOperationException("Oyun oturumu bulunamadı veya süresi doldu.");
    private async Task<PublicGameQuestion?> NextAsync(State s)
    {
        var all=await db.GameQuestions.AsNoTracking().Include(x=>x.Options).Where(x=>x.IsActive&&x.GameType==GameType.OperationArena&&x.GradeLevel==s.Grade&&(s.Topic=="Karışık"||x.Topic==s.Topic)&&!s.Used.Contains(x.Id)).ToListAsync();
        var target=s.Correct>=3&&s.Correct-s.Wrong>=2?GameDifficulty.Hard:s.Wrong>=2&&s.Wrong>s.Correct?GameDifficulty.Easy:s.Difficulty;
        var q=all.Where(x=>x.DifficultyLevel==target).OrderBy(_=>Guid.NewGuid()).FirstOrDefault()??all.OrderBy(_=>Guid.NewGuid()).FirstOrDefault();
        var result=q is null?null:new PublicGameQuestion(q.Id,q.QuestionText,q.Options.OrderBy(_=>Guid.NewGuid()).Select(x=>x.OptionText).ToArray(),s.Used.Count+1);s.CurrentQuestion=result;return result;
    }
    private static string Normalize(string value)=>value.Trim().Replace(',','.').ToUpperInvariant();
    private static string Key(string token)=>"public-game:"+token;
    private sealed class State
    {
        public int Grade{get;init;}public string Topic{get;init;}="Karışık";public GameDifficulty Difficulty{get;init;}public int DurationSeconds{get;init;}public DateTime StartedAt{get;init;}
        public int CurrentQuestionId{get;set;}public PublicGameQuestion? CurrentQuestion{get;set;}public HashSet<int> Used{get;}=[];public int Score{get;set;}public int Correct{get;set;}public int Wrong{get;set;}public int Streak{get;set;}public int Longest{get;set;}public decimal TotalSeconds{get;set;}public bool Completed{get;set;}
    }
}
public record PublicGameQuestion(int Id,string Text,IReadOnlyList<string> Options,int Number);
public record PublicGameAnswer(bool IsCorrect,string CorrectAnswer,string Explanation,int EarnedScore,int TotalScore,int Correct,int Wrong,int Streak,PublicGameQuestion? NextQuestion,bool Finished);
public record PublicGameResult(int Score,int Correct,int Wrong,decimal Accuracy,decimal AverageAnswerTime,int LongestStreak,int Grade,string Topic,GameDifficulty Difficulty,int DurationSeconds);
