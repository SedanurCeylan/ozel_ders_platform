using Microsoft.AspNetCore.Mvc;
using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Services.Games;

namespace OzelDersYonetim.Controllers;

public class GamesController(PublicGameService games):Controller
{
    public IActionResult Index()=>View();
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int grade,string topic,GameDifficulty difficulty,int durationSeconds)
    {
        try{var game=await games.StartAsync(grade,topic,difficulty,durationSeconds);TempData["PublicGameToken"]=game.Token;return RedirectToAction(nameof(Play),new{token=game.Token});}
        catch(InvalidOperationException ex){TempData["GameError"]=ex.Message;return RedirectToAction(nameof(Index));}
    }
    public IActionResult Play(string token)
    {
        if(!Owns(token))return NotFound();try{var game=games.Current(token);ViewBag.Token=token;ViewBag.Question=game.Question;ViewBag.EndUnixMs=new DateTimeOffset(DateTime.SpecifyKind(game.EndsAt,DateTimeKind.Utc)).ToUnixTimeMilliseconds();return View();}catch(InvalidOperationException){return RedirectToAction(nameof(Index));}
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer([FromBody]PublicAnswerRequest request)
    {
        if(!Owns(request.Token))return NotFound();try{return Json(await games.AnswerAsync(request.Token,request.QuestionId,request.Answer,request.AnswerTimeSeconds));}catch(InvalidOperationException ex){return BadRequest(new{message=ex.Message});}
    }
    public IActionResult Complete(string token)
    {
        if(!Owns(token))return NotFound();try{return View("Result",games.Complete(token));}catch(InvalidOperationException){return RedirectToAction(nameof(Index));}
    }
    private bool Owns(string token)=>!string.IsNullOrWhiteSpace(token)&&string.Equals(TempData.Peek("PublicGameToken") as string,token,StringComparison.Ordinal);
}
public record PublicAnswerRequest(string Token,int QuestionId,string Answer,decimal AnswerTimeSeconds);
