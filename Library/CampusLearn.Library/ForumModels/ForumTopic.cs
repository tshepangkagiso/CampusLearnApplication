namespace CampusLearn.Library.ForumModels;

public class ForumTopic
{
    [Key]
    public int ForumTopicID { get; set; }

    [Required]
    [StringLength(200)]
    public string ForumTopicTitle { get; set; } = string.Empty;

    [Required]
    public string ForumTopicDescription { get; set; } = string.Empty;

    [Required]
    public string RelatedModuleCode { get; set; } = string.Empty;

    public int TopicUpVote { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public DateTime TopicCreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivity { get; set; }

    // Anonymous posting support
    public int? UserProfileID { get; set; } // Reference to User Management service
    public bool IsAnonymous { get; set; } = false;
    public string? AnonymousName { get; set; }

    // Moderation flags
    public bool IsLocked { get; set; } = false;
    public bool IsPinned { get; set; } = false;
    public bool IsFeatured { get; set; } = false;

    // Navigation property (within this service)
    public ICollection<ForumTopicResponse> Responses { get; set; } = new List<ForumTopicResponse>();
}
