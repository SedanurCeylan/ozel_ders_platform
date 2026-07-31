using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Services.Documents;

namespace OzelDersYonetim.Areas.Student.Controllers;

[Area("Student"), Authorize(Roles = IdentityDataSeeder.StudentRole)]
public class DocumentsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IStudentDocumentService documentService) : Controller
{
    public async Task<IActionResult> Index(string? category)
    {
        var userId = userManager.GetUserId(User); var studentId = await dbContext.StudentProfiles.Where(x => x.ApplicationUserId == userId && x.IsActive).Select(x => (int?)x.Id).SingleOrDefaultAsync(); if (!studentId.HasValue) return NotFound();
        var query = dbContext.CourseDocuments.AsNoTracking().Where(x => x.IsActive && (x.AccessType != DocumentAccessType.SelectedStudents || x.StudentDocuments.Any(s => s.StudentProfileId == studentId.Value)));
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
        ViewBag.Category = category; ViewBag.Categories = await query.Select(x => x.Category).Distinct().OrderBy(x => x).ToListAsync();
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }
    public async Task<IActionResult> Download(int id) { var result = await documentService.OpenForStudentAsync(id, userManager.GetUserId(User)!); return result is null ? NotFound() : File(result.Value.Stream, result.Value.ContentType, result.Value.FileName); }
}
