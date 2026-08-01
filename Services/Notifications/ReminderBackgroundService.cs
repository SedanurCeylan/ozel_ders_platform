using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OzelDersYonetim.Configuration;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.Notifications;

namespace OzelDersYonetim.Services.Notifications;

public class ReminderBackgroundService(IServiceScopeFactory scopeFactory, IOptions<ReminderOptions> options, ILogger<ReminderBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled) return;
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Max(1, options.Value.CheckIntervalMinutes)));
            do
            {
                try { await CheckAsync(stoppingToken); }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex) { logger.LogError(ex,"Hatırlatma kontrolü tamamlanamadı."); }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
    }

    private async Task CheckAsync(CancellationToken token)
    {
        using var scope=scopeFactory.CreateScope(); var db=scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); var notify=scope.ServiceProvider.GetRequiredService<INotificationService>(); var email=scope.ServiceProvider.GetRequiredService<IEmailService>(); var templates=scope.ServiceProvider.GetRequiredService<IEmailTemplateService>(); var now=DateTime.Now;
        var assignments=await db.StudentAssignments.Include(x=>x.Assignment).Include(x=>x.StudentProfile).ThenInclude(x=>x.ApplicationUser).Where(x=>x.Status<StudentAssignmentStatus.Submitted&&x.StudentProfile.ApplicationUserId!=null).ToListAsync(token);
        foreach(var item in assignments)
        {
            var hours=(item.DueDate-now).TotalHours; string? key=hours switch { >=23.5 and <=24.5=>"Ödev-24-saat", >=2.5 and <=3.5=>"Ödev-3-saat", <0=>"Ödev-gecikti", _=>null }; if(key is null)continue;
            await DispatchAsync(db,notify,email,templates,key,"Ödev",item.Id,item.StudentProfile.ApplicationUserId!,item.StudentProfile.Email,item.StudentProfile.FullName,hours<0?"Ödevinizin teslim süresi geçti":"Ödev teslim tarihi yaklaşıyor",$"{item.Assignment.Title} ödevinin son teslim tarihi: {item.DueDate:dd.MM.yyyy HH:mm}",item.DueDate,$"/Student/Assignments/Details/{item.Id}",token);
        }
        var lessons=await db.StudentLessons.Include(x=>x.Lesson).Include(x=>x.StudentProfile).ThenInclude(x=>x.ApplicationUser).Where(x=>x.Lesson.StartDate>now&&x.Lesson.Status!=LessonStatus.TeacherCancelled&&x.Lesson.Status!=LessonStatus.StudentCancelled&&x.StudentProfile.ApplicationUserId!=null).ToListAsync(token);
        foreach(var item in lessons)
        {
            var hours=(item.Lesson.StartDate-now).TotalHours; string? key=hours switch { >=23.5 and <=24.5=>"Ders-24-saat", >=.5 and <=1.5=>"Ders-1-saat", _=>null }; if(key is null)continue;
            await DispatchAsync(db,notify,email,templates,key,"Ders",item.Id,item.StudentProfile.ApplicationUserId!,item.StudentProfile.Email,item.StudentProfile.FullName,"Dersiniz yaklaşıyor",$"{item.Lesson.Title} dersi {item.Lesson.StartDate:dd.MM.yyyy HH:mm} tarihinde başlayacak.",item.Lesson.StartDate,$"/Student/Lessons/Details/{item.Id}",token);
        }
    }

    private static async Task DispatchAsync(ApplicationDbContext db,INotificationService notify,IEmailService email,IEmailTemplateService templates,string key,string entity,int entityId,string userId,string address,string name,string title,string message,DateTime date,string url,CancellationToken token)
    {
        if(await db.ReminderDispatches.AnyAsync(x=>x.ReminderType==key&&x.EntityType==entity&&x.EntityId==entityId&&x.ApplicationUserId==userId,token))return;
        db.ReminderDispatches.Add(new ReminderDispatch{ReminderType=key,EntityType=entity,EntityId=entityId,ApplicationUserId=userId});await db.SaveChangesAsync(token);
        await notify.CreateAsync(new[]{userId},title,message,key,entityId,url);await email.SendEmailAsync(address,name,title,templates.Build(title,message,date),key,entityId);
    }
}
