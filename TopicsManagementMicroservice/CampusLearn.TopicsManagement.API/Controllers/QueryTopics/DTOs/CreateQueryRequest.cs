namespace CampusLearn.TopicsManagement.API.Controllers.QueryTopics.DTOs;

public class CreateQueryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public int StudentId { get; set; }
}
