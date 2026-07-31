namespace OzelDersYonetim.Configuration;
public class ReminderOptions { public const string SectionName="Reminders"; public int CheckIntervalMinutes { get; set; }=15; public bool Enabled { get; set; }=true; }
