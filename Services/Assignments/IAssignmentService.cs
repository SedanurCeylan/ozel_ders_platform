using Microsoft.AspNetCore.Http;
using OzelDersYonetim.Models.Assignments;

namespace OzelDersYonetim.Services.Assignments;

public interface IAssignmentService
{
    Task CreateAndAssignAsync(Assignment assignment, IReadOnlyCollection<int> studentIds, IFormFile? attachment);
}
