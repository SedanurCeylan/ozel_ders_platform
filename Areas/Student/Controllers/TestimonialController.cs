using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.Identity;
namespace OzelDersYonetim.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles=IdentityDataSeeder.StudentRole)]
public class TestimonialController(ApplicationDbContext db,UserManager<ApplicationUser> users):Controller
{
 public async Task<IActionResult> Index(){var student=await CurrentStudent();if(student is null)return NotFound();return View(await db.StudentTestimonials.AsNoTracking().SingleOrDefaultAsync(x=>x.StudentProfileId==student.Id)??new StudentTestimonial{StudentProfileId=student.Id,Rating=5});}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Index(StudentTestimonial input){var student=await CurrentStudent();if(student is null)return NotFound();if(!ModelState.IsValid){input.StudentProfileId=student.Id;return View(input);}var item=await db.StudentTestimonials.SingleOrDefaultAsync(x=>x.StudentProfileId==student.Id);if(item is null){item=new StudentTestimonial{StudentProfileId=student.Id,CreatedAt=DateTime.UtcNow};db.StudentTestimonials.Add(item);}item.Comment=input.Comment.Trim();item.Rating=input.Rating;item.IsActive=false;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();TempData["Success"]="Yorumunuz kaydedildi. Yönetici onayından sonra sitede yayınlanacak.";return RedirectToAction(nameof(Index));}
 private async Task<Models.Students.StudentProfile?> CurrentStudent(){var userId=users.GetUserId(User);return await db.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.ApplicationUserId==userId&&x.IsActive);}
}
