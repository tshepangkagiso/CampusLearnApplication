namespace CampusLearn.TopicsManagement.API.Controllers.QueryResponses.DTOs;

public class UpdateTutorResponseRequest
{
    public string? Comment { get; set; }
    public IFormFile? MediaContent { get; set; }
    public bool IsSolution { get; set; }
}
