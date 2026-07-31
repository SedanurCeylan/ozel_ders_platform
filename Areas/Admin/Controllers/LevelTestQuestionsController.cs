using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"),Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class LevelTestQuestionsController(ApplicationDbContext db):Controller
{
    public async Task<IActionResult> Index(int? grade)
    {
        var query=db.GameQuestions.AsNoTracking().Where(x=>x.GameType==GameType.LevelTest);if(grade.HasValue)query=query.Where(x=>x.GradeLevel==grade);ViewBag.Grade=grade;
        return View(await query.OrderBy(x=>x.GradeLevel).ThenBy(x=>x.Topic).ThenBy(x=>x.Id).ToListAsync());
    }
    public IActionResult Create()=>Form(new GameQuestion{GameType=GameType.LevelTest,QuestionType=GameQuestionType.MultipleChoice,Score=100,TimeLimitSeconds=60});
    [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Create(GameQuestion model,string optionA,string optionB,string optionC,string optionD)
    {
        model.GameType=GameType.LevelTest;model.QuestionType=GameQuestionType.MultipleChoice;var options=Options(optionA,optionB,optionC,optionD);Validate(model,options);if(!ModelState.IsValid)return Form(model,options);
        model.Options=options.Select((x,i)=>new GameQuestionOption{OptionText=x,IsCorrect=Same(x,model.CorrectAnswer),DisplayOrder=i}).ToList();db.GameQuestions.Add(model);await db.SaveChangesAsync();TempData["Success"]="Mini test sorusu eklendi.";return RedirectToAction(nameof(Index),new{grade=model.GradeLevel});
    }
    public async Task<IActionResult> Edit(int id)
    {
        var item=await db.GameQuestions.Include(x=>x.Options).SingleOrDefaultAsync(x=>x.Id==id&&x.GameType==GameType.LevelTest);return item is null?NotFound():Form(item,item.Options.OrderBy(x=>x.DisplayOrder).Select(x=>x.OptionText).ToList());
    }
    [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Edit(int id,GameQuestion model,string optionA,string optionB,string optionC,string optionD)
    {
        if(id!=model.Id)return BadRequest();var options=Options(optionA,optionB,optionC,optionD);Validate(model,options);if(!ModelState.IsValid)return Form(model,options);var item=await db.GameQuestions.Include(x=>x.Options).SingleOrDefaultAsync(x=>x.Id==id&&x.GameType==GameType.LevelTest);if(item is null)return NotFound();
        item.GradeLevel=model.GradeLevel;item.Topic=model.Topic.Trim();item.DifficultyLevel=model.DifficultyLevel;item.QuestionText=model.QuestionText.Trim();item.CorrectAnswer=model.CorrectAnswer.Trim();item.Explanation=model.Explanation.Trim();item.IsActive=model.IsActive;item.UpdatedAt=DateTime.UtcNow;db.GameQuestionOptions.RemoveRange(item.Options);item.Options=options.Select((x,i)=>new GameQuestionOption{OptionText=x,IsCorrect=Same(x,item.CorrectAnswer),DisplayOrder=i}).ToList();await db.SaveChangesAsync();TempData["Success"]="Mini test sorusu güncellendi.";return RedirectToAction(nameof(Index),new{grade=item.GradeLevel});
    }
    [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Toggle(int id)
    {
        var item=await db.GameQuestions.SingleOrDefaultAsync(x=>x.Id==id&&x.GameType==GameType.LevelTest);if(item is null)return NotFound();item.IsActive=!item.IsActive;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();TempData["Success"]=item.IsActive?"Soru aktifleştirildi.":"Soru pasifleştirildi.";return RedirectToAction(nameof(Index),new{grade=item.GradeLevel});
    }
    private IActionResult Form(GameQuestion model,IReadOnlyList<string>? options=null){options??=[];ViewBag.OptionA=options.ElementAtOrDefault(0);ViewBag.OptionB=options.ElementAtOrDefault(1);ViewBag.OptionC=options.ElementAtOrDefault(2);ViewBag.OptionD=options.ElementAtOrDefault(3);return View(model.Id==0?"Create":"Edit",model);}
    private void Validate(GameQuestion model,IReadOnlyList<string> options){if(options.Count<2)ModelState.AddModelError(string.Empty,"En az iki seçenek yazın.");if(!options.Any(x=>Same(x,model.CorrectAnswer)))ModelState.AddModelError(nameof(model.CorrectAnswer),"Doğru cevap seçeneklerden biri olmalıdır.");}
    private static List<string> Options(params string[] values)=>values.Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    private static bool Same(string left,string right)=>string.Equals(left.Trim(),right.Trim(),StringComparison.OrdinalIgnoreCase);
}
