namespace CampusLearn.ForumManagement.API.Controllers.Forum_Topic;

// DTOs
public class CreateForumTopicRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public int UserProfileID { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string? AnonymousName { get; set; }
}
