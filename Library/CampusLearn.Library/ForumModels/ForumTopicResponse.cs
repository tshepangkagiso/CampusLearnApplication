namespace CampusLearn.Library.ForumModels;

public class ForumTopicResponse
{
    [Key]
    public int ResponseID { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;

    [Url]
    public string? MediaContentUrl { get; set; }

    public int ResponseUpVote { get; set; } = 0;
    public DateTime ResponseCreationDate { get; set; } = DateTime.UtcNow;

    // Anonymous posting support
    public int? UserProfileID { get; set; } // Reference to User Management service
    public bool IsAnonymous { get; set; } = false;
    public string? AnonymousName { get; set; }

    // Foreign key
    public int ForumTopicID { get; set; }

    // Navigation property (within this service)
    public ForumTopic ForumTopic { get; set; } = null!;
}
