using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class GamesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.GameSessions.AsNoTracking().Include(x => x.StudentProfile).Where(x => x.IsCompleted).OrderByDescending(x => x.CompletedAt).Take(100).ToListAsync());
    public async Task<IActionResult> Questions(int? grade, string? topic) { var query = db.GameQuestions.AsNoTracking(); if (grade.HasValue) query = query.Where(x => x.GradeLevel == grade); if (!string.IsNullOrWhiteSpace(topic)) query = query.Where(x => x.Topic.Contains(topic)); ViewBag.Grade = grade; ViewBag.Topic = topic; return View(await query.OrderBy(x => x.GradeLevel).ThenBy(x => x.Topic).ThenBy(x => x.DifficultyLevel).ToListAsync()); }
    public IActionResult CreateQuestion() => View(new GameQuestion());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateQuestion(GameQuestion model, string optionA, string optionB, string optionC, string optionD) { var options = new[] { optionA, optionB, optionC, optionD }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(); if (model.QuestionType == GameQuestionType.MultipleChoice && (options.Count < 2 || !options.Any(x => x.Trim() == model.CorrectAnswer.Trim()))) ModelState.AddModelError(nameof(model.CorrectAnswer), "Doğru cevap seçeneklerden biri olmalıdır."); if (!ModelState.IsValid) return View(model); model.Options = options.Select((x, i) => new GameQuestionOption { OptionText = x.Trim(), IsCorrect = x.Trim() == model.CorrectAnswer.Trim(), DisplayOrder = i }).ToList(); db.GameQuestions.Add(model); await db.SaveChangesAsync(); TempData["Success"] = "Oyun sorusu eklendi."; return RedirectToAction(nameof(Questions)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> ToggleQuestion(int id) { var q = await db.GameQuestions.FindAsync(id); if (q is null) return NotFound(); q.IsActive = !q.IsActive; q.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return RedirectToAction(nameof(Questions)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> DeleteSession(int id) { var item = await db.GameSessions.FindAsync(id); if (item is null) return NotFound(); db.GameSessions.Remove(item); await db.SaveChangesAsync(); TempData["Success"] = "Oyun sonucu silindi."; return RedirectToAction(nameof(Index)); }
}
