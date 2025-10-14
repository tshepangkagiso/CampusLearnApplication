namespace CampusLearn.TopicsManagement.API.Controllers.QueryResponses.DTOs;

// DTOs
public class CreateTutorResponseRequest
{
    public string Comment { get; set; } = string.Empty;
    public IFormFile? MediaContent { get; set; }
    public bool IsSolution { get; set; } = false;
}
