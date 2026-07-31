using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
namespace OzelDersYonetim.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class AuditLogsController(ApplicationDbContext dbContext):Controller
{
    public async Task<IActionResult> Index(string? actionType,string? entityType){var q=dbContext.AuditLogs.AsNoTracking().OrderByDescending(x=>x.CreatedAt).AsQueryable();if(!string.IsNullOrWhiteSpace(actionType))q=q.Where(x=>x.ActionType==actionType);if(!string.IsNullOrWhiteSpace(entityType))q=q.Where(x=>x.EntityType==entityType);ViewBag.ActionType=actionType;ViewBag.EntityType=entityType;ViewBag.Actions=await dbContext.AuditLogs.Select(x=>x.ActionType).Distinct().OrderBy(x=>x).ToListAsync();ViewBag.Entities=await dbContext.AuditLogs.Select(x=>x.EntityType).Distinct().OrderBy(x=>x).ToListAsync();return View(await q.Take(500).ToListAsync());}
}
