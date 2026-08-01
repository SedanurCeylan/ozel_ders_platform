using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Assignments;

namespace OzelDersYonetim.Areas.Student.Controllers;

[Area("Student"), Authorize(Roles = IdentityDataSeeder.StudentRole)]
public class AssignmentsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IAssignmentSubmissionService submissionService, AssignmentFileService assignmentFiles) : Controller
{
    public async Task<IActionResult> Index(string? scope)
    {
        var userId = userManager.GetUserId(User);
        var query = dbContext.StudentAssignments.AsNoTracking().Include(x => x.Assignment).Where(x => x.StudentProfile.ApplicationUserId == userId);
        query = scope switch { "completed" => query.Where(x => x.Status == StudentAssignmentStatus.Evaluated || x.Status == StudentAssignmentStatus.Completed), "late" => query.Where(x => x.DueDate < DateTime.Now && x.Status < StudentAssignmentStatus.Submitted), _ => query.Where(x => x.Status != StudentAssignmentStatus.Completed) };
        ViewBag.Scope = scope;
        return View(await query.OrderBy(x => x.DueDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = userManager.GetUserId(User);
        var item = await dbContext.StudentAssignments.Include(x => x.Assignment).Include(x => x.Submissions).SingleOrDefaultAsync(x => x.Id == id && x.StudentProfile.ApplicationUserId == userId);
        if (item is null) return NotFound();
        if (item.ViewedAt is null) { item.ViewedAt = DateTime.UtcNow; if (item.Status == StudentAssignmentStatus.Assigned) item.Status = StudentAssignmentStatus.Viewed; await dbContext.SaveChangesAsync(); }
        return View(new AssignmentSubmissionViewModel { StudentAssignment = item });
    }

    public async Task<IActionResult> DownloadAttachment(int id)
    {
        var userId = userManager.GetUserId(User);
        var item = await dbContext.StudentAssignments.AsNoTracking().Include(x => x.Assignment)
            .SingleOrDefaultAsync(x => x.Id == id && x.StudentProfile.ApplicationUserId == userId);
        if (item?.Assignment.AttachmentPath is null) return NotFound();
        var file = assignmentFiles.Open(item.Assignment.AttachmentPath, item.AssignmentId);
        return file is null ? NotFound() : File(file.Value.Stream, file.Value.ContentType, "odev-eki" + Path.GetExtension(item.Assignment.AttachmentPath));
    }

    public async Task<IActionResult> DownloadSubmission(int id)
    {
        var userId = userManager.GetUserId(User);
        var item = await dbContext.AssignmentSubmissions.AsNoTracking()
            .Include(x => x.StudentAssignment)
            .SingleOrDefaultAsync(x => x.Id == id && x.StudentAssignment.StudentProfile.ApplicationUserId == userId);
        if (item?.FilePath is null) return NotFound();
        var file = assignmentFiles.Open(item.FilePath, item.StudentAssignment.AssignmentId, item.StudentAssignment.StudentProfileId);
        return file is null ? NotFound() : File(file.Value.Stream, file.Value.ContentType, item.FileName ?? "teslim" + Path.GetExtension(item.FilePath));
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> Submit(int id, AssignmentSubmissionViewModel model)
    {
        var userId = userManager.GetUserId(User);
        var item = await dbContext.StudentAssignments.Include(x => x.Assignment).Include(x => x.Submissions).SingleOrDefaultAsync(x => x.Id == id && x.StudentProfile.ApplicationUserId == userId);
        if (item is null) return NotFound();
        try { await submissionService.SubmitAsync(item, model.StudentDescription, model.File); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); model.StudentAssignment = item; return View("Details", model); }
        TempData["Success"] = "Ödeviniz başarıyla teslim edildi.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
