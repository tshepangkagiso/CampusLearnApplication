namespace CampusLearn.TopicsManagement.API.Controllers.FAQ;

// DTOs
public class CreateFAQRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
}
