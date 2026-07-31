using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Controllers;

[Route("seviye-testi")]
public class LevelTestController(ApplicationDbContext db,IMemoryCache cache):Controller
{
    [HttpGet("")]
    public IActionResult Index()=>View();

    [HttpPost("baslat"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int grade)
    {
        if(grade is <5 or >8)return BadRequest();
        var all=await db.GameQuestions.AsNoTracking().Include(x=>x.Options).Where(x=>x.IsActive&&x.GameType==GameType.LevelTest&&x.GradeLevel==grade&&x.Options.Count>=2).ToListAsync();
        var questions=all.OrderBy(_=>Guid.NewGuid()).Take(5).ToList();if(questions.Count<5){TempData["TestError"]="Bu sınıf için seviye testi hazırlamaya yetecek soru bulunmuyor.";return RedirectToAction(nameof(Index));}
        var token=Guid.NewGuid().ToString("N");cache.Set("level-test:"+token,new LevelTestState(grade,questions.Select(x=>x.Id).ToArray()),TimeSpan.FromMinutes(20));
        return View("Test",new LevelTestViewModel{Token=token,Grade=grade,Questions=questions});
    }

    [HttpPost("sonuc"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Result(string token,Dictionary<int,string> answers)
    {
        if(!cache.TryGetValue("level-test:"+token,out LevelTestState? state)||state is null)return RedirectToAction(nameof(Index));
        var questions=await db.GameQuestions.AsNoTracking().Where(x=>state.QuestionIds.Contains(x.Id)).ToListAsync();var correctTopics=new List<string>();var wrongTopics=new List<string>();var correct=0;
        foreach(var q in questions){var isCorrect=answers.TryGetValue(q.Id,out var answer)&&Normalize(answer)==Normalize(q.CorrectAnswer);if(isCorrect){correct++;correctTopics.Add(q.Topic);}else wrongTopics.Add(q.Topic);}
        cache.Remove("level-test:"+token);var wrong=questions.Count-correct;var accuracy=Math.Round(correct*100m/questions.Count,1);var recommendation=accuracy switch{>=80=>"İleri düzey soru çözümü ve yeni nesil problem çalışmalarına odaklanabilirsiniz.",>=50=>"Konu tekrarını düzenli soru çözümüyle birlikte ilerletmeniz uygun olur.",_=>"Temel kazanımları öğretmen desteğiyle adım adım güçlendirmeniz önerilir."};
        return View(new LevelTestResultViewModel(state.Grade,correct,wrong,accuracy,correctTopics.Distinct().ToList(),wrongTopics.Distinct().ToList(),recommendation));
    }
    private static string Normalize(string value)=>value.Trim().Replace(',','.').ToUpperInvariant();
    private sealed record LevelTestState(int Grade,int[] QuestionIds);
}
