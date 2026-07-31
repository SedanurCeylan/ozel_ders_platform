using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Games;

namespace OzelDersYonetim.Areas.Student.Controllers;

[Area("Student"), Authorize(Roles = IdentityDataSeeder.StudentRole)]
public class GamesController(ApplicationDbContext db, UserManager<ApplicationUser> users, IGameSessionService sessions) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = users.GetUserId(User)!;
        var student = await db.StudentProfiles.AsNoTracking().SingleAsync(x => x.ApplicationUserId == userId);
        var history = await db.GameSessions.AsNoTracking().Where(x => x.StudentProfileId == student.Id && x.IsCompleted).OrderByDescending(x => x.CompletedAt).Take(8).ToListAsync();
        var grade = int.TryParse(new string(student.GradeLevel.TakeWhile(char.IsDigit).ToArray()), out var value) ? Math.Clamp(value, 5, 8) : 5;
        return View(new GamesHomeViewModel { DefaultGrade = grade, RecentSessions = history, TotalGames = history.Count, TotalScore = history.Sum(x => x.TotalScore), Accuracy = history.Count == 0 ? 0 : Math.Round(history.Average(x => x.AccuracyPercentage), 1) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> StartArena(int grade, string topic, GameDifficulty difficulty, int durationSeconds)
    {
        try { var session = await sessions.StartArenaAsync(users.GetUserId(User)!, grade, topic, difficulty, durationSeconds); return RedirectToAction(nameof(PlayArena), new { id = session.Id }); }
        catch (InvalidOperationException ex) { TempData["Success"] = ex.Message; return RedirectToAction(nameof(Index)); }
    }

    public async Task<IActionResult> PlayArena(int id)
    {
        try
        {
            var question = await sessions.GetNextQuestionAsync(users.GetUserId(User)!, id);
            if (question is null) return RedirectToAction(nameof(Complete), new { id });
            ViewBag.Question = question; ViewBag.SessionId = id;
            var session = await sessions.GetOwnedResultAsync(users.GetUserId(User)!, id);
            ViewBag.Duration = session!.DurationSeconds;
            var utcStart = DateTime.SpecifyKind(session.StartedAt, DateTimeKind.Utc);
            ViewBag.EndUnixMs = new DateTimeOffset(utcStart.AddSeconds(session.DurationSeconds)).ToUnixTimeMilliseconds();
            return View();
        }
        catch (UnauthorizedAccessException) { return NotFound(); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer([FromBody] GameAnswerRequest request)
    {
        try { return Json(await sessions.AnswerAsync(users.GetUserId(User)!, request)); }
        catch (UnauthorizedAccessException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var completed = await sessions.CompleteAsync(users.GetUserId(User)!, id);
            if (completed.TotalQuestions == 0)
            {
                db.GameSessions.Remove(completed);
                await db.SaveChangesAsync();
                TempData["Success"] = "Oyun başlamadan sona erdi. Yeni bir oyun başlatabilirsiniz.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Result), new { id });
        }
        catch (UnauthorizedAccessException) { return NotFound(); }
    }

    public async Task<IActionResult> Result(int id)
    {
        var userId = users.GetUserId(User)!; var result = await sessions.GetOwnedResultAsync(userId, id); if (result is null || !result.IsCompleted) return NotFound();
        var previous = await db.GameSessions.AsNoTracking().Where(x => x.StudentProfile.ApplicationUserId == userId && x.GameType == result.GameType && x.IsCompleted && x.Id != id).MaxAsync(x => (int?)x.TotalScore) ?? 0;
        return View(new GameResultViewModel { Session = result, PreviousBest = previous });
    }

}
