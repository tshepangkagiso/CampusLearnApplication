namespace CampusLearn.TopicsManagement.API.Controllers.FAQ.DTOs;

public class UpdateFAQRequest
{
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string? ModuleCode { get; set; }
    public bool IsPublished { get; set; }
}
