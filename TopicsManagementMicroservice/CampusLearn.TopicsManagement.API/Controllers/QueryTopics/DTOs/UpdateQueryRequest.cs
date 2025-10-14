namespace CampusLearn.TopicsManagement.API.Controllers.QueryTopics.DTOs;

// Update DTO
public class UpdateQueryRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsResolved { get; set; }
    public bool IsUrgent { get; set; }
}
