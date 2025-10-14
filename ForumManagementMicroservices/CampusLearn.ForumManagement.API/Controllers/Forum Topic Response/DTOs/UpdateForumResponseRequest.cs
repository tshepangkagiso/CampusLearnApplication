namespace CampusLearn.ForumManagement.API.Controllers.Forum_Topic_Response;

public class UpdateForumResponseRequest
{
    public string? Comment { get; set; }
    public IFormFile? MediaContent { get; set; }
}