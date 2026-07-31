using Microsoft.AspNetCore.Http;
using OzelDersYonetim.Models.Assignments;

namespace OzelDersYonetim.Services.Assignments;

public interface IAssignmentSubmissionService
{
    Task SubmitAsync(StudentAssignment studentAssignment, string? description, IFormFile? file);
}
