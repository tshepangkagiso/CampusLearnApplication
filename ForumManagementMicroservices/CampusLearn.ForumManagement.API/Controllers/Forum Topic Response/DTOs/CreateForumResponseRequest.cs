namespace CampusLearn.ForumManagement.API.Controllers.Forum_Topic_Response;

// DTOs
public class CreateForumResponseRequest
{
    public string Comment { get; set; } = string.Empty;
    public IFormFile? MediaContent { get; set; }
    public int UserProfileID { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string? AnonymousName { get; set; } = "Anonymous User";
}
