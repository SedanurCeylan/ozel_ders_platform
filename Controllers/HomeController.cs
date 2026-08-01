using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.Games;
using OzelDersYonetim.Services;

namespace OzelDersYonetim.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly StoragePathResolver _storage;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, StoragePathResolver storage)
    {
        _logger = logger;
        _dbContext = dbContext;
        _storage = storage;
    }

    public async Task<IActionResult> Index()
    {
        var model = await PageAsync("Home");
        model.ClassContents = await _dbContext.ContentSections.AsNoTracking().Where(x=>x.PageKey=="SecondaryEducation"&&x.SectionKey.StartsWith("grade")&&x.IsActive).OrderBy(x=>x.DisplayOrder).Take(4).ToListAsync();
        model.PublicDocuments = await _dbContext.CourseDocuments.AsNoTracking().Where(x => x.IsActive && x.AccessType == DocumentAccessType.Public).OrderByDescending(x => x.CreatedAt).Take(6).ToListAsync();
        model.StudentTestimonials = await _dbContext.StudentTestimonials.AsNoTracking().Include(x=>x.StudentProfile).Where(x=>x.IsActive&&x.StudentProfile.IsActive).OrderByDescending(x=>x.UpdatedAt).Take(6).ToListAsync();
        var dailyFacts = await _dbContext.DailyFacts.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.DisplayOrder).ThenBy(x=>x.Id).ToListAsync();
        if(dailyFacts.Count>0)
        {
            var dayNumber=DateOnly.FromDateTime(DateTime.Today).DayNumber;
            model.DailyFact=dailyFacts[dayNumber%dailyFacts.Count];
        }
        return View(model);
    }

    public async Task<IActionResult> About()
    {
        return View(await PageAsync("About"));
    }

    public IActionResult Privacy()
    {
        return RedirectToAction(nameof(About));
    }

    public async Task<IActionResult> Trainings()
    {
        return View(await PageAsync("Trainings"));
    }

    public async Task<IActionResult> SecondaryEducation()
    {
        return View(await PageAsync("SecondaryEducation"));
    }

    public async Task<IActionResult> PublicDocument(int id)
    {
        var document = await _dbContext.CourseDocuments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.IsActive && x.AccessType == DocumentAccessType.Public);
        if (document is null) return NotFound();
        var path = _storage.ResolveStoredFile(document.StoredFilePath, "documents");
        if (path is null || !System.IO.File.Exists(path)) return NotFound();
        return File(System.IO.File.OpenRead(path), document.ContentType, document.OriginalFileName);
    }

    [Route("Home/StatusCode/{statusCode:int}")]
    public IActionResult StatusCodePage(int statusCode)
    {
        Response.StatusCode = statusCode;
        ViewData["StatusCode"] = statusCode;
        ViewData["Title"] = statusCode == 404 ? "Sayfa Bulunamadı" : "Bir Sorun Oluştu";
        return View("StatusCode");
    }

    private async Task<PublicSiteViewModel> PageAsync(string pageKey) => new()
    {
        Settings = await _dbContext.SiteSettings.AsNoTracking().SingleAsync(),
        Sections = await _dbContext.ContentSections.AsNoTracking().Where(x => x.PageKey == pageKey && x.IsActive).OrderBy(x => x.DisplayOrder).ToListAsync()
    };

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
