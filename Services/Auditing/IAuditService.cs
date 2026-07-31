namespace OzelDersYonetim.Services.Auditing;
public interface IAuditService { Task LogAsync(string actionType,string entityType,int? entityId,string description); }
